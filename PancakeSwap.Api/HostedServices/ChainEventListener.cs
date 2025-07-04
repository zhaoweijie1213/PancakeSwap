using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using PancakeSwap.Application.Services;
using PancakeSwap.Infrastructure.Blockchain.PancakePredictionV2;
using PancakeSwap.Infrastructure.Blockchain.PancakePredictionV2.ContractDefinition;
using PancakeSwap.Api.Hubs;
using PancakeSwap.Infrastructure.Database;
using PancakeSwap.Application.Database.Entities;
using PancakeSwap.Application.Enums;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace PancakeSwap.Api.HostedServices
{
    /// <summary>
    /// 监听 PancakePrediction 合约的 EndRound 事件。
    /// </summary>
    /// <remarks>
    /// 初始化 <see cref="ChainEventListener"/> 实例。
    /// </remarks>
    /// <param name="configuration">配置读取器。</param>
    /// <param name="web3">Web3 实例。</param>
    /// <param name="roundService">回合服务。</param>
    /// <param name="dbContext">数据库上下文</param>
    /// <param name="logger">日志组件。</param>
    /// <param name="hubContext"></param>
    public class ChainEventListener(
        IConfiguration configuration,
        IWeb3 web3,
        IRoundService roundService,
        ApplicationDbContext dbContext,
        ILogger<ChainEventListener> logger,
        IHubContext<PredictionHub> hubContext) : BackgroundService
    {
        private readonly IWeb3 _web3 = web3;
        private readonly IRoundService _roundService = roundService;
        private readonly ILogger<ChainEventListener> _logger = logger;
        private readonly IHubContext<PredictionHub> _hubContext = hubContext;
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly string _contractAddress =
            (configuration["PREDICTION_ADDRESS"] ?? configuration["CONTRACT_ADDR_LOCAL"] ?? string.Empty).Trim();

        /// <summary>
        /// PancakePredictionV2 合约服务实例。
        /// </summary>
        private readonly PancakePredictionV2Service _predictionService =
            new(web3,
                (configuration["PREDICTION_ADDRESS"] ?? configuration["CONTRACT_ADDR_LOCAL"] ?? string.Empty).Trim());

        /// <summary>
        /// 回合结束事件过滤器 ID。
        /// </summary>
        private HexBigInteger? _endRoundFilterId;

        /// <summary>
        /// 回合结束事件定义。
        /// </summary>
        private Event<EndRoundEventDTO>? _endRoundEvent;

        /// <summary>
        /// 多头下注事件过滤器 ID。
        /// </summary>
        private HexBigInteger? _betBullFilterId;

        /// <summary>
        /// 多头下注事件定义。
        /// </summary>
        private Event<BetBullEventDTO>? _betBullEvent;

        /// <summary>
        /// 下跌下注事件过滤器 ID。
        /// </summary>
        private HexBigInteger? _betBearFilterId;

        /// <summary>
        /// 下跌下注事件定义。
        /// </summary>
        private Event<BetBearEventDTO>? _betBearEvent;

        /// <summary>
        /// 锁仓事件过滤器 ID。
        /// </summary>
        private HexBigInteger? _lockRoundFilterId;

        /// <summary>
        /// 锁仓事件定义。
        /// </summary>
        private Event<LockRoundEventDTO>? _lockRoundEvent;

        /// <summary>
        /// 领奖事件过滤器 ID。
        /// </summary>
        private HexBigInteger? _claimFilterId;

        /// <summary>
        /// 领奖事件定义。
        /// </summary>
        private Event<ClaimEventDTO>? _claimEvent;

        /// <summary>
        /// 在后台任务启动时初始化事件过滤器。
        /// </summary>
        /// <param name="cancellationToken">取消标记。</param>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_contractAddress))
            {
                _logger.LogWarning("PREDICTION_ADDRESS not configured");
                return;
            }

            var earliest = BlockParameter.CreateEarliest();

            _endRoundEvent = _web3.Eth.GetEvent<EndRoundEventDTO>(_contractAddress);
            _endRoundFilterId = await _endRoundEvent.CreateFilterAsync(earliest);

            _betBullEvent = _web3.Eth.GetEvent<BetBullEventDTO>(_contractAddress);
            _betBullFilterId = await _betBullEvent.CreateFilterAsync(earliest);

            _betBearEvent = _web3.Eth.GetEvent<BetBearEventDTO>(_contractAddress);
            _betBearFilterId = await _betBearEvent.CreateFilterAsync(earliest);

            _lockRoundEvent = _web3.Eth.GetEvent<LockRoundEventDTO>(_contractAddress);
            _lockRoundFilterId = await _lockRoundEvent.CreateFilterAsync(earliest);

            _claimEvent = _web3.Eth.GetEvent<ClaimEventDTO>(_contractAddress);
            _claimFilterId = await _claimEvent.CreateFilterAsync(earliest);

            await base.StartAsync(cancellationToken);
        }

        /// <summary>
        /// 循环监听并处理合约事件。
        /// </summary>
        /// <param name="stoppingToken">取消标记。</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (string.IsNullOrWhiteSpace(_contractAddress))
            {
                _logger.LogWarning("PREDICTION_ADDRESS not configured");
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    //锁仓事件入库
                    if (_lockRoundEvent != null && _lockRoundFilterId != null)
                    {
                        var logs = await _lockRoundEvent.GetFilterChangesAsync(_lockRoundFilterId);
                        foreach (var log in logs)
                        {
                            var epoch = (long)log.Event.Epoch;
                            await SyncRoundAsync(epoch, stoppingToken);
                        }
                    }

                    //回合结束入库
                    if (_endRoundEvent != null && _endRoundFilterId != null)
                    {
                        var logs = await _endRoundEvent.GetFilterChangesAsync(_endRoundFilterId);
                        foreach (var log in logs)
                        {
                            var epoch = (long)log.Event.Epoch;
                            await SyncRoundAsync(epoch, stoppingToken);
                            await _hubContext.Clients.All.SendAsync("roundEnded", new { id = epoch }, stoppingToken);
                        }
                    }

                    //多头下注入库
                    if (_betBullEvent != null && _betBullFilterId != null)
                    {
                        var logs = await _betBullEvent.GetFilterChangesAsync(_betBullFilterId);
                        foreach (var log in logs)
                        {
                            await SaveBetAsync(log.Event.Sender, (long)log.Event.Epoch, log.Event.Amount, Position.Up);
                        }
                    }

                    //下跌下注事件入库
                    if (_betBearEvent != null && _betBearFilterId != null)
                    {
                        var logs = await _betBearEvent.GetFilterChangesAsync(_betBearFilterId);
                        foreach (var log in logs)
                        {
                            await SaveBetAsync(log.Event.Sender, (long)log.Event.Epoch, log.Event.Amount, Position.Down);
                        }
                    }

                    //领奖事件入库
                    if (_claimEvent != null && _claimFilterId != null)
                    {
                        var logs = await _claimEvent.GetFilterChangesAsync(_claimFilterId);
                        foreach (var log in logs)
                        {
                            await SaveClaimAsync(log.Event.Sender, (long)log.Event.Epoch, log.Event.Amount, log.Log.TransactionHash);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing events");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 停止服务时卸载所有过滤器。
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_endRoundFilterId != null)
            {
                await _web3.Eth.Filters.UninstallFilter.SendRequestAsync(_endRoundFilterId);
            }

            if (_betBullFilterId != null)
            {
                await _web3.Eth.Filters.UninstallFilter.SendRequestAsync(_betBullFilterId);
            }

            if (_betBearFilterId != null)
            {
                await _web3.Eth.Filters.UninstallFilter.SendRequestAsync(_betBearFilterId);
            }

            if (_lockRoundFilterId != null)
            {
                await _web3.Eth.Filters.UninstallFilter.SendRequestAsync(_lockRoundFilterId);
            }

            if (_claimFilterId != null)
            {
                await _web3.Eth.Filters.UninstallFilter.SendRequestAsync(_claimFilterId);
            }

            await base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// 保存下注记录到数据库。
        /// </summary>
        /// <param name="user"></param>
        /// <param name="epoch"></param>
        /// <param name="amountWei"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private async Task SaveBetAsync(string user, long epoch, System.Numerics.BigInteger amountWei, Position position)
        {
            var amount = (decimal)amountWei / 1_000000000000000000m;
            var bet = new BetEntity
            {
                Epoch = epoch,
                UserAddress = user,
                Amount = amount,
                Position = position,
                Claimed = false,
                Reward = 0m
            };
            await _dbContext.Db.Insertable(bet).ExecuteCommandAsync();
        }

        /// <summary>
        /// 保存认领奖励记录到数据库
        /// </summary>
        /// <param name="user"></param>
        /// <param name="epoch"></param>
        /// <param name="rewardWei"></param>
        /// <param name="txHash"></param>
        /// <returns></returns>
        private async Task SaveClaimAsync(string user, long epoch, System.Numerics.BigInteger rewardWei, string txHash)
        {
            var reward = (decimal)rewardWei / 1_000000000000000000m;
            var claim = new ClaimEntity
            {
                Epoch = epoch,
                UserAddress = user,
                Reward = reward,
                TxHash = txHash
            };
            await _dbContext.Db.Insertable(claim).ExecuteCommandAsync();

            await _dbContext.Db.Updateable<BetEntity>()
                .SetColumns(b => new BetEntity { Claimed = true, Reward = reward })
                .Where(b => b.Epoch == epoch && b.UserAddress == user)
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 从链上同步指定回合的数据到数据库。
        /// </summary>
        /// <param name="epoch">回合编号。</param>
        /// <param name="ct">取消标记。</param>
        private async Task SyncRoundAsync(long epoch, CancellationToken ct)
        {
            var dto = await _predictionService.RoundsQueryAsync(epoch);

            var entity = new RoundEntity
            {
                Id = (long)dto.Epoch,
                StartTime = DateTimeOffset.FromUnixTimeSeconds((long)dto.StartTimestamp).UtcDateTime,
                LockTime = DateTimeOffset.FromUnixTimeSeconds((long)dto.LockTimestamp).UtcDateTime,
                CloseTime = DateTimeOffset.FromUnixTimeSeconds((long)dto.CloseTimestamp).UtcDateTime,
                LockPrice = (decimal)dto.LockPrice / 1_00000000m,
                ClosePrice = (decimal)dto.ClosePrice / 1_00000000m,
                LockOracleId = (long)dto.LockOracleId,
                CloseOracleId = (long)dto.CloseOracleId,
                TotalAmount = (decimal)dto.TotalAmount / 1_000000000000000000m,
                BullAmount = (decimal)dto.BullAmount / 1_000000000000000000m,
                BearAmount = (decimal)dto.BearAmount / 1_000000000000000000m,
                RewardBaseCalAmount = (decimal)dto.RewardBaseCalAmount / 1_000000000000000000m,
                RewardAmount = (decimal)dto.RewardAmount / 1_000000000000000000m,
                Status = dto.OracleCalled
                    ? RoundStatus.Ended
                    : dto.LockPrice != 0 ? RoundStatus.Locked : RoundStatus.Live
            };

            var exists = await _dbContext.Db.Queryable<RoundEntity>()
                .Where(r => r.Id == entity.Id)
                .FirstAsync();

            if (exists == null)
            {
                await _dbContext.Db.Insertable(entity).ExecuteCommandAsync();
            }
            else
            {
                await _dbContext.Db.Updateable(entity)
                    .Where(r => r.Id == entity.Id)
                    .ExecuteCommandAsync();
            }
        }
    }
}
