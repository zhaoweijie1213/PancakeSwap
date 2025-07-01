using System;

namespace PancakeSwap.Application.Output
{
    /// <summary>
    /// 当前回合详情。
    /// </summary>
    public class CurrentRoundOutput
    {
        /// <summary>
        /// 回合编号。
        /// </summary>
        public long Epoch { get; set; }

        /// <summary>
        /// 距离结束剩余秒数。
        /// </summary>
        public int SecondsRemaining { get; set; }

        /// <summary>
        /// 看涨总额。
        /// </summary>
        public decimal BullAmount { get; set; }

        /// <summary>
        /// 看跌总额。
        /// </summary>
        public decimal BearAmount { get; set; }
    }
}
