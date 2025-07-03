using QYQ.Base.Common.IOCExtensions;
using System.Threading;
using System.Threading.Tasks;

namespace PancakeSwap.Application.Services
{
    /// <summary>
    /// 定义回合相关的业务操作。
    /// </summary>
    public interface IRoundService : ISingletonDependency
    {
        /// <summary>
        /// 创建下一回合并返回其 Epoch。
        /// </summary>
        /// <returns>新回合的 Epoch。</returns>
        Task<long> CreateNextRoundAsync();

        /// <summary>
        /// 锁定指定回合并写入锁仓价。
        /// </summary>
        /// <param name="epoch">回合编号。</param>
        Task LockRoundAsync(long epoch);

        /// <summary>
        /// 结算指定回合并写入收盘价与状态。
        /// </summary>
        /// <param name="epoch">回合编号。</param>
        Task SettleRoundAsync(long epoch);

        /// <summary>
        /// 获取当前回合信息。
        /// </summary>
        Task<Output.CurrentRoundOutput?> GetCurrentRoundAsync();

        /// <summary>
        /// 获取最近结束的回合记录。
        /// </summary>
        /// <param name="count">需要的回合数量。</param>
        Task<List<Output.HistoryRoundOutput>> GetHistoryAsync(int count);

        /// <summary>
        /// 获取即将开始的回合列表。
        /// </summary>
        /// <param name="count">需要的回合数量。</param>
        Task<List<Output.UpcomingRoundOutput>> GetUpcomingAsync(int count);

        /// <summary>
        /// 获取图表数据。
        /// </summary>
        Task<Output.ChartDataOutput> GetChartDataAsync();
    }
}
