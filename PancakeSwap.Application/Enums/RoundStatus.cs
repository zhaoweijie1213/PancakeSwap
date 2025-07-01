namespace PancakeSwap.Application.Enums
{
    /// <summary>
    /// 回合状态。
    /// </summary>
    public enum RoundStatus
    {
        /// <summary>
        /// 即将开始。
        /// </summary>
        Upcoming = 0,

        /// <summary>
        /// 回合进行中。
        /// </summary>
        Live = 1,

        /// <summary>
        /// 已锁定，等待结算。
        /// </summary>
        Locked = 2,

        /// <summary>
        /// 已结束。
        /// </summary>
        Ended = 3
    }
}
