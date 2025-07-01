using System;
using SqlSugar;

namespace PancakeSwap.Infrastructure.Database.Entities
{
    /// <summary>
    ///     Represents a prediction round.
    /// </summary>
    [SugarTable("rounds")]
    public class RoundEntity
    {
        /// <summary>
        ///     Epoch identifier.
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false, ColumnName = "epoch")]
        public long Epoch { get; set; }

        [SugarColumn(ColumnName = "start_time")]
        public DateTime StartTime { get; set; }

        [SugarColumn(ColumnName = "lock_time")]
        public DateTime LockTime { get; set; }

        [SugarColumn(ColumnName = "close_time")]
        public DateTime CloseTime { get; set; }

        [SugarColumn(ColumnName = "lock_price", ColumnDataType = "decimal(18,8)")]
        public decimal LockPrice { get; set; }

        [SugarColumn(ColumnName = "close_price", ColumnDataType = "decimal(18,8)")]
        public decimal ClosePrice { get; set; }

        [SugarColumn(ColumnName = "status")]
        public int Status { get; set; }

        [SugarColumn(ColumnName = "total_amount", ColumnDataType = "decimal(18,8)")]
        public decimal TotalAmount { get; set; }

        [SugarColumn(ColumnName = "bull_amount", ColumnDataType = "decimal(18,8)")]
        public decimal BullAmount { get; set; }

        [SugarColumn(ColumnName = "bear_amount", ColumnDataType = "decimal(18,8)")]
        public decimal BearAmount { get; set; }

        /// <summary>
        /// 获胜方向，0 表示看涨，1 表示看跌。
        /// </summary>
        [SugarColumn(ColumnName = "winning_position", IsNullable = true)]
        public int? WinningPosition { get; set; }
    }
}
