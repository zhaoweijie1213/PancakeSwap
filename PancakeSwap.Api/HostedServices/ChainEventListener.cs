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
using PancakeSwap.Infrastructure.Blockchain.PancakePredictionV2.ContractDefinition;
using PancakeSwap.Api.Hubs;
using PancakeSwap.Infrastructure.Database;
using PancakeSwap.Application.Database.Entities;
using PancakeSwap.Application.Enums;

namespace PancakeSwap.Api.HostedServices
{
    /// <summary>
    /// 监听 PancakePrediction 合约的 EndRound 事件。
    /// </summary>
    public class ChainEventListener : BackgroundService
    {
        private readonly IWeb3 _web3;
        private readonly IRoundService _roundService;
        private readonly ILogger<ChainEventListener> _logger;
        private readonly IHubContext<PredictionHub> _hubContext;
        private readonly ApplicationDbContext _dbContext;
        private readonly string _contractAddress;
        private HexBigInteger? _endRoundFilterId;
        private Nethereum.Contracts.Event<EndRoundEventDTO>? _endRoundEvent;
        private HexBigInteger? _betBullFilterId;
        private Nethereum.Contracts.Event<BetBullEventDTO>? _betBullEvent;
        private HexBigInteger? _betBearFilterId;
        private Nethereum.Contracts.Event<BetBearEventDTO>? _betBearEvent;
        private HexBigInteger? _claimFilterId;
        private Nethereum.Contracts.Event<ClaimEventDTO>? _claimEvent;

        /// <summary>
        /// 初始化 <see cref="ChainEventListener"/> 实例。
        /// </summary>
        /// <param name="configuration">配置读取器。</param>
        /// <param name="web3">Web3 实例。</param>
        /// <param name="roundService">回合服务。</param>
        /// <param name="logger">日志组件。</param>
        /// <param name="hubContext"></param>
        public ChainEventListener(
            IConfiguration configuration,
            IWeb3 web3,
            IRoundService roundService,
            ApplicationDbContext dbContext,
            ILogger<ChainEventListener> logger,
            IHubContext<PredictionHub> hubContext)
        {
            _web3 = web3;
            _roundService = roundService;
            _dbContext = dbContext;
            _logger = logger;
            _hubContext = hubContext;
            _contractAddress = configuration.GetValue<string>("PREDICTION_ADDRESS") ?? string.Empty;
        }

        /// <summary>
        /// 监听合约事件并处理相关逻辑。
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (string.IsNullOrWhiteSpace(_contractAddress))
            {
                _logger.LogWarning("PREDICTION_ADDRESS not configured");
                return;
            }

            //回合结束事件
            _endRoundEvent = _web3.Eth.GetEvent<EndRoundEventDTO>(_contractAddress);
            _endRoundFilterId = await _endRoundEvent.CreateFilterAsync();

            //多头下注事件
            _betBullEvent = _web3.Eth.GetEvent<BetBullEventDTO>(_contractAddress);
            _betBullFilterId = await _betBullEvent.CreateFilterAsync();

            //下跌下注事件
            _betBearEvent = _web3.Eth.GetEvent<BetBearEventDTO>(_contractAddress);
            _betBearFilterId = await _betBearEvent.CreateFilterAsync();

            //认领奖励事件
            _claimEvent = _web3.Eth.GetEvent<ClaimEventDTO>(_contractAddress);
            // 创建认领奖励事件过滤器
            _claimFilterId = await _claimEvent.CreateFilterAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_endRoundEvent != null && _endRoundFilterId != null)
                    {
                        var logs = await _endRoundEvent.GetFilterChangesAsync(_endRoundFilterId);
                        foreach (var log in logs)
                        {
                            var epoch = (long)log.Event.Epoch;
                            await _roundService.SettleRoundAsync(epoch, stoppingToken);
                            await _hubContext.Clients.All.SendAsync("roundEnded", new { id = epoch }, stoppingToken);
                        }
                    }

                    if (_betBullEvent != null && _betBullFilterId != null)
                    {
                        var logs = await _betBullEvent.GetFilterChangesAsync(_betBullFilterId);
                        foreach (var log in logs)
                        {
                            await SaveBetAsync(log.Event.Sender, (long)log.Event.Epoch, log.Event.Amount, Position.Up);
                        }
                    }

                    if (_betBearEvent != null && _betBearFilterId != null)
                    {
                        var logs = await _betBearEvent.GetFilterChangesAsync(_betBearFilterId);
                        foreach (var log in logs)
                        {
                            await SaveBetAsync(log.Event.Sender, (long)log.Event.Epoch, log.Event.Amount, Position.Down);
                        }
                    }

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

        /// <inheritdoc />
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

            if (_claimFilterId != null)
            {
                await _web3.Eth.Filters.UninstallFilter.SendRequestAsync(_claimFilterId);
            }

            await base.StopAsync(cancellationToken);
        }

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
    }
}
