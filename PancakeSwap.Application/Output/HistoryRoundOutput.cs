using System;

namespace PancakeSwap.Application.Output
{
    /// <summary>
    /// 结束回合数据。
    /// </summary>
    public class HistoryRoundOutput
    {
        /// <summary>
        /// 回合编号。
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 锁仓价格。
        /// </summary>
        public decimal LockPrice { get; set; }

        /// <summary>
        /// 收盘价格。
        /// </summary>
        public decimal ClosePrice { get; set; }

        /// <summary>
        /// 总下注金额。
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 押"上升"的总金额。
        /// </summary>
        public decimal BullAmount { get; set; }

        /// <summary>
        /// 押"下降"的总金额。
        /// </summary>
        public decimal BearAmount { get; set; }

        /// <summary>
        /// 可分配奖金池。
        /// </summary>
        public decimal RewardAmount { get; set; }

        /// <summary>
        /// 回合结束时间。
        /// </summary>
        public long EndTime { get; set; }

        /// <summary>
        /// 回合状态。
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 上升赔率。
        /// </summary>
        public decimal OddsUp { get; set; }

        /// <summary>
        /// 下降赔率。
        /// </summary>
        public decimal OddsDown { get; set; }
    }
}
