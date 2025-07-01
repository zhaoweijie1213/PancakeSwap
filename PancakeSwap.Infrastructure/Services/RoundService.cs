using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Nethereum.Web3;
using PancakeSwap.Application.Database.Entities;
using PancakeSwap.Application.Enums;
using PancakeSwap.Application.Output;
using PancakeSwap.Application.Services;
using PancakeSwap.Infrastructure.Database;
using SqlSugar;

namespace PancakeSwap.Infrastructure.Services
{
    /// <summary>
    /// 回合业务实现。
    /// </summary>
    public class RoundService : IRoundService
    {
        private const string LatestAnswerAbi =
            "[{\"inputs\":[],\"name\":\"latestAnswer\",\"outputs\":[{\"internalType\":\"int256\",\"name\":\"\",\"type\":\"int256\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";

        private readonly ApplicationDbContext _context;
        private readonly IWeb3 _web3;
        private readonly string _oracleAddress;

        /// <summary>
        /// 初始化服务实例。
        /// </summary>
        /// <param name="context">数据库上下文。</param>
        /// <param name="web3">Web3 实例。</param>
        /// <param name="configuration">配置读取实例。</param>
        public RoundService(ApplicationDbContext context, IWeb3 web3, IConfiguration configuration)
        {
            _context = context;
            _web3 = web3;
            _oracleAddress = configuration.GetValue<string>("CHAINLINK_ORACLE");
        }

        /// <inheritdoc />
        public async Task<long> CreateNextRoundAsync(CancellationToken ct)
        {
            var last = await _context.Db.Queryable<RoundEntity>()
                .OrderBy(r => r.Epoch, OrderByType.Desc)
                .FirstAsync();
            var nextEpoch = (last?.Epoch ?? 0) + 1;

            var round = new RoundEntity
            {
                Epoch = nextEpoch,
                StartTime = DateTime.UtcNow,
                Status = (int)RoundStatus.Pending
            };

            await _context.Db.Insertable(round).ExecuteCommandAsync();
            return nextEpoch;
        }

        /// <inheritdoc />
        public async Task LockRoundAsync(long epoch, CancellationToken ct)
        {
            var price = await FetchLatestPriceAsync();

            await _context.Db.Updateable<RoundEntity>()
                .SetColumns(r => new RoundEntity
                {
                    LockPrice = price,
                    LockTime = DateTime.UtcNow,
                    Status = (int)RoundStatus.Locked
                })
                .Where(r => r.Epoch == epoch)
                .ExecuteCommandAsync();
        }

        /// <inheritdoc />
        public async Task SettleRoundAsync(long epoch, CancellationToken ct)
        {
            var closePrice = await FetchLatestPriceAsync();
            var round = await _context.Db.Queryable<RoundEntity>()
                .Where(r => r.Epoch == epoch)
                .FirstAsync();
            if (round == null)
            {
                return;
            }

            await _context.Db.Updateable<RoundEntity>()
                .SetColumns(r => new RoundEntity
                {
                    ClosePrice = closePrice,
                    CloseTime = DateTime.UtcNow,
                    Status = (int)RoundStatus.Ended
                })
                .Where(r => r.Epoch == epoch)
                .ExecuteCommandAsync();
        }

        /// <inheritdoc />
        public async Task<CurrentRoundOutput?> GetCurrentRoundAsync(CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var round = await _context.Db.Queryable<RoundEntity>()
                .Where(r => r.Status != (int)RoundStatus.Ended)
                .OrderBy(r => r.Epoch, OrderByType.Desc)
                .FirstAsync();

            if (round == null)
            {
                return null;
            }

            var secondsRemaining = (int)Math.Max(0, (round.CloseTime - now).TotalSeconds);
            return new CurrentRoundOutput
            {
                Epoch = round.Epoch,
                SecondsRemaining = secondsRemaining,
                BullAmount = round.BullAmount,
                BearAmount = round.BearAmount
            };
        }

        /// <inheritdoc />
        public async Task<List<HistoryRoundOutput>> GetHistoryAsync(int count, CancellationToken ct)
        {
            var rounds = await _context.Db.Queryable<RoundEntity>()
                .Where(r => r.Status == (int)RoundStatus.Ended)
                .OrderBy(r => r.Epoch, OrderByType.Desc)
                .Take(count)
                .ToListAsync();

            var list = new List<HistoryRoundOutput>();
            foreach (var r in rounds)
            {
                var total = r.TotalAmount;
                var oddsUp = r.BullAmount > 0 ? total / r.BullAmount : 0m;
                var oddsDown = r.BearAmount > 0 ? total / r.BearAmount : 0m;
                list.Add(new HistoryRoundOutput
                {
                    Id = r.Epoch,
                    LockPrice = r.LockPrice,
                    ClosePrice = r.ClosePrice,
                    TotalAmount = total,
                    UpAmount = r.BullAmount,
                    DownAmount = r.BearAmount,
                    RewardAmount = total,
                    EndTime = new DateTimeOffset(r.CloseTime).ToUnixTimeSeconds(),
                    Status = RoundStatus.Ended.ToString().ToLowerInvariant(),
                    OddsUp = oddsUp,
                    OddsDown = oddsDown
                });
            }

            return list;
        }

        /// <inheritdoc />
        public async Task<List<UpcomingRoundOutput>> GetUpcomingAsync(int count, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var rounds = await _context.Db.Queryable<RoundEntity>()
                .Where(r => r.Status == (int)RoundStatus.Pending && r.StartTime > now)
                .OrderBy(r => r.Epoch, OrderByType.Asc)
                .Take(count)
                .ToListAsync();

            var list = new List<UpcomingRoundOutput>();
            foreach (var r in rounds)
            {
                list.Add(new UpcomingRoundOutput
                {
                    Id = r.Epoch,
                    StartTime = new DateTimeOffset(r.StartTime).ToUnixTimeSeconds(),
                    EndTime = new DateTimeOffset(r.CloseTime).ToUnixTimeSeconds()
                });
            }

            return list;
        }

        /// <inheritdoc />
        public async Task<ChartDataOutput> GetChartDataAsync(CancellationToken ct)
        {
            var since = DateTime.UtcNow.AddMinutes(-10);
            var rounds = await _context.Db.Queryable<RoundEntity>()
                .Where(r => r.CloseTime > since)
                .OrderBy(r => r.CloseTime)
                .ToListAsync();

            var output = new ChartDataOutput();
            foreach (var r in rounds)
            {
                output.OriginalXData.Add(r.CloseTime.ToString("HH:mm:ss"));
                output.SeriesData.Add(r.ClosePrice);
            }

            return output;
        }

        private async Task<decimal> FetchLatestPriceAsync()
        {
            var contract = _web3.Eth.GetContract(LatestAnswerAbi, _oracleAddress);
            var function = contract.GetFunction("latestAnswer");
            var value = await function.CallAsync<BigInteger>();
            return (decimal)value / 1_00000000m;
        }
    }
}
