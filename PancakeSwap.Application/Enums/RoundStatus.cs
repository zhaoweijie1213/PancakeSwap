namespace PancakeSwap.Application.Enums
{
    /// <summary>
    /// 表示回合的当前状态。
    /// </summary>
    public enum RoundStatus
    {
        /// <summary>
        /// 已创建但尚未锁定。
        /// </summary>
        Pending = 0,

        /// <summary>
        /// 已锁定，等待结算。
        /// </summary>
        Locked = 1,

        /// <summary>
        /// 已结束并写入收盘价。
        /// </summary>
        Ended = 2
    }
}
