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
    /// 定时推送图表数据的后台服务。
    /// </summary>
    public class ChartBroadcastService : BackgroundService
    {
        private readonly IHubContext<PredictionHub> _hubContext;
        private readonly IRoundService _roundService;
        private readonly ILogger<ChartBroadcastService> _logger;

        /// <summary>
        /// 初始化实例。
        /// </summary>
        public ChartBroadcastService(IHubContext<PredictionHub> hubContext, IRoundService roundService, ILogger<ChartBroadcastService> logger)
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
                    var data = await _roundService.GetChartDataAsync(stoppingToken);
                    await _hubContext.Clients.All.SendAsync("chartData", data, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to broadcast chart data");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }
}
