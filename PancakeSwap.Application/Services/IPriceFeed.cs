using System.Threading;
using System.Threading.Tasks;

namespace PancakeSwap.Application.Services
{
    /// <summary>
    /// 提供价格信息的接口。
    /// </summary>
    public interface IPriceFeed
    {
        /// <summary>
        /// 获取最新价格。
        /// </summary>
        /// <param name="ct">取消标记。</param>
        /// <returns>最新价格，若无法获取则为 null。</returns>
        Task<decimal?> GetLatestPriceAsync(CancellationToken ct);
    }
}
