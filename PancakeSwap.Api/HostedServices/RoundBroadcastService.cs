using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PancakeSwap.Api.Hubs;
using PancakeSwap.Application.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PancakeSwap.Api.HostedServices
{
    /// <summary>
    /// 定时向客户端广播当前回合信息的后台服务。
    /// </summary>
    public class RoundBroadcastService : BackgroundService
    {
        private readonly IHubContext<PredictionHub> _hubContext;
        private readonly IRoundService _roundService;
        private readonly ILogger<RoundBroadcastService> _logger;

        /// <summary>
        /// 初始化实例。
        /// </summary>
        public RoundBroadcastService(IHubContext<PredictionHub> hubContext, IRoundService roundService, ILogger<RoundBroadcastService> logger)
        {
            _hubContext = hubContext;
            _roundService = roundService;
            _logger = logger;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var info = await _roundService.GetCurrentRoundAsync(stoppingToken);
                    if (info != null)
                    {
                        await _hubContext.Clients.All.SendAsync("currentRound", info, stoppingToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to broadcast round info");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }
}
