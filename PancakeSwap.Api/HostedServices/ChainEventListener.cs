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
        private readonly string _contractAddress;
        private Nethereum.Hex.HexTypes.HexBigInteger? _filterId;
        private Nethereum.Contracts.Event<EndRoundEventDTO>? _event;

        /// <summary>
        /// 初始化 <see cref="ChainEventListener"/> 实例。
        /// </summary>
        /// <param name="configuration">配置读取器。</param>
        /// <param name="web3">Web3 实例。</param>
        /// <param name="roundService">回合服务。</param>
        /// <param name="logger">日志组件。</param>
        public ChainEventListener(
            IConfiguration configuration,
            IWeb3 web3,
            IRoundService roundService,
            ILogger<ChainEventListener> logger,
            IHubContext<PredictionHub> hubContext)
        {
            _web3 = web3;
            _roundService = roundService;
            _logger = logger;
            _hubContext = hubContext;
            _contractAddress = configuration.GetValue<string>("PREDICTION_ADDRESS") ?? string.Empty;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (string.IsNullOrWhiteSpace(_contractAddress))
            {
                _logger.LogWarning("PREDICTION_ADDRESS not configured");
                return;
            }

            _event = _web3.Eth.GetEvent<EndRoundEventDTO>(_contractAddress);
            _filterId = await _event.CreateFilterAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var logs = await _event.GetFilterChangesAsync(_filterId);
                    foreach (var log in logs)
                    {
                        var epoch = (long)log.Event.Epoch;
                        await _roundService.SettleRoundAsync(epoch, stoppingToken);
                        await _hubContext.Clients.All.SendAsync("roundEnded", new { id = epoch }, stoppingToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing EndRound events");
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
            if (_event != null && _filterId != null)
            {
                try
                {
                    await _web3.Eth.Filters.UninstallFilter.SendRequestAsync(_filterId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to uninstall EndRound filter");
                }
            }

            await base.StopAsync(cancellationToken);
        }
    }
}
