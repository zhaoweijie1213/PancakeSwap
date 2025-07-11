using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly ApplicationDbContext _context;
        private readonly IPriceFeed _priceFeed;

        /// <summary>
        /// 初始化服务实例。
        /// </summary>
        /// <param name="context">数据库上下文。</param>
        /// <param name="priceFeed">价格源。</param>
        public RoundService(ApplicationDbContext context, IPriceFeed priceFeed)
        {
            _context = context;
            _priceFeed = priceFeed;
        }

        /// <inheritdoc />
        public async Task<long> CreateNextRoundAsync()
        {
            var last = await _context.Db.Queryable<RoundEntity>()
                .OrderBy(r => r.Id, OrderByType.Desc)
                .FirstAsync();
            var nextId = (last?.Id ?? 0) + 1;

            var round = new RoundEntity
            {
                Id = nextId,
                StartTime = DateTime.UtcNow,
                Status = RoundStatus.Upcoming
            };

            await _context.Db.Insertable(round).ExecuteCommandAsync();
            return nextId;
        }

        /// <summary>
        /// 锁定指定回合并写入锁仓价和时间。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task LockRoundAsync(long id)
        {
            var price = await _priceFeed.GetLatestPriceAsync(CancellationToken.None);
            if (price == null)
            {
                throw new InvalidOperationException("Price unavailable");
            }

            await _context.Db.Updateable<RoundEntity>()
                .SetColumns(r => new RoundEntity
                {
                    LockPrice = price.Value,
                    LockTime = DateTime.UtcNow,
                    Status = RoundStatus.Locked
                })
                .Where(r => r.Id == id)
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 写入回合结算信息，包括收盘价和状态。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task SettleRoundAsync(long id)
        {
            var closePrice = await _priceFeed.GetLatestPriceAsync(CancellationToken.None);
            if (closePrice == null)
            {
                throw new InvalidOperationException("Price unavailable");
            }
            var round = await _context.Db.Queryable<RoundEntity>()
                .Where(r => r.Id == id)
                .FirstAsync();
            if (round == null)
            {
                return;
            }

            await _context.Db.Updateable<RoundEntity>()
                .SetColumns(r => new RoundEntity
                {
                    ClosePrice = closePrice.Value,
                    CloseTime = DateTime.UtcNow,
                    Status = RoundStatus.Ended
                })
                .Where(r => r.Id == id)
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取当前回合信息。
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<CurrentRoundOutput?> GetCurrentRoundAsync()
        {
            var now = DateTime.UtcNow;
            var round = await _context.Db.Queryable<RoundEntity>()
                .Where(r => r.Status != RoundStatus.Ended)
                .OrderBy(r => r.Id, OrderByType.Desc)
                .FirstAsync();

            if (round == null)
            {
                return null;
            }

            var secondsRemaining = (int)Math.Max(0, (round.CloseTime - now).TotalSeconds);
            return new CurrentRoundOutput
            {
                Epoch = round.Id,
                SecondsRemaining = secondsRemaining,
                BullAmount = round.BullAmount,
                BearAmount = round.BearAmount
            };
        }

        /// <summary>
        /// 获取历史回合记录。
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<List<HistoryRoundOutput>> GetHistoryAsync(int count)
        {
            var rounds = await _context.Db.Queryable<RoundEntity>()
                .Where(r => r.Status == RoundStatus.Ended)
                .OrderBy(r => r.Id, OrderByType.Desc)
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
                    Id = r.Id,
                    LockPrice = r.LockPrice,
                    ClosePrice = r.ClosePrice,
                    TotalAmount = total,
                    BullAmount = r.BullAmount,
                    BearAmount = r.BearAmount,
                    RewardAmount = total,
                    EndTime = new DateTimeOffset(r.CloseTime).ToUnixTimeSeconds(),
                    Status = RoundStatus.Ended.ToString().ToLowerInvariant(),
                    OddsUp = oddsUp,
                    OddsDown = oddsDown
                });
            }

            return list;
        }

        /// <summary>
        /// 获取即将开始的回合列表。
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<List<UpcomingRoundOutput>> GetUpcomingAsync(int count)
        {
            var now = DateTime.UtcNow;
            var rounds = await _context.Db.Queryable<RoundEntity>()
                .Where(r => r.Status == RoundStatus.Upcoming && r.StartTime > now)
                .OrderBy(r => r.Id, OrderByType.Asc)
                .Take(count)
                .ToListAsync();

            var list = new List<UpcomingRoundOutput>();
            foreach (var r in rounds)
            {
                list.Add(new UpcomingRoundOutput
                {
                    Id = r.Id,
                    StartTime = new DateTimeOffset(r.StartTime).ToUnixTimeSeconds(),
                    EndTime = new DateTimeOffset(r.CloseTime).ToUnixTimeSeconds()
                });
            }

            return list;
        }

        /// <summary>
        /// 获取图表数据，包含最近10分钟内的回合收盘价格。
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<ChartDataOutput> GetChartDataAsync()
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

    }
}
