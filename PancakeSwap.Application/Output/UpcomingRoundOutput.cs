using System;

namespace PancakeSwap.Application.Output
{
    /// <summary>
    /// 即将开始的回合信息。
    /// </summary>
    public class UpcomingRoundOutput
    {
        /// <summary>
        /// 回合编号。
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 开始时间时间戳。
        /// </summary>
        public long StartTime { get; set; }

        /// <summary>
        /// 结束时间时间戳。
        /// </summary>
        public long EndTime { get; set; }
    }
}
