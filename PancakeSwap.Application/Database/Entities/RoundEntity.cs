using System;
using SqlSugar;

namespace PancakeSwap.Application.Database.Entities
{
    /// <summary>
    /// 表示链上预测游戏的回合记录。
    /// </summary>
    [SugarTable("round")]
    public class RoundEntity
    {
        /// <summary>
        /// 回合编号。
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false, ColumnName = "epoch")]
        public long Epoch { get; set; }

        /// <summary>
        /// 回合开始时间。
        /// </summary>
        [SugarColumn(ColumnName = "start_time")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 锁仓时间。
        /// </summary>
        [SugarColumn(ColumnName = "lock_time")]
        public DateTime LockTime { get; set; }

        /// <summary>
        /// 回合结束时间。
        /// </summary>
        [SugarColumn(ColumnName = "close_time")]
        public DateTime CloseTime { get; set; }

        /// <summary>
        /// 锁仓价格。
        /// </summary>
        [SugarColumn(ColumnName = "lock_price", ColumnDataType = "decimal(18,8)")]
        public decimal LockPrice { get; set; }

        /// <summary>
        /// 结束价格。
        /// </summary>
        [SugarColumn(ColumnName = "close_price", ColumnDataType = "decimal(18,8)")]
        public decimal ClosePrice { get; set; }

        /// <summary>
        /// 回合状态。
        /// </summary>
        [SugarColumn(ColumnName = "status")]
        public int Status { get; set; }

        /// <summary>
        /// 本回合的下注总金额。
        /// </summary>
        [SugarColumn(ColumnName = "total_amount", ColumnDataType = "decimal(18,8)")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 押涨总金额。
        /// </summary>
        [SugarColumn(ColumnName = "bull_amount", ColumnDataType = "decimal(18,8)")]
        public decimal BullAmount { get; set; }

        /// <summary>
        /// 押跌总金额。
        /// </summary>
        [SugarColumn(ColumnName = "bear_amount", ColumnDataType = "decimal(18,8)")]
        public decimal BearAmount { get; set; }

        /// <summary>
        /// 获胜方向，0 表示看涨，1 表示看跌。
        /// </summary>
        [SugarColumn(ColumnName = "winning_position", IsNullable = true)]
        public int? WinningPosition { get; set; }
    }
}
