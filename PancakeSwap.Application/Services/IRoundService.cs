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
        /// <param name="ct">取消标记。</param>
        /// <returns>新回合的 Epoch。</returns>
        Task<long> CreateNextRoundAsync(CancellationToken ct);

        /// <summary>
        /// 锁定指定回合并写入锁仓价。
        /// </summary>
        /// <param name="epoch">回合编号。</param>
        /// <param name="ct">取消标记。</param>
        Task LockRoundAsync(long epoch, CancellationToken ct);

        /// <summary>
        /// 结算指定回合并写入收盘价与状态。
        /// </summary>
        /// <param name="epoch">回合编号。</param>
        /// <param name="ct">取消标记。</param>
        Task SettleRoundAsync(long epoch, CancellationToken ct);
    }
}
