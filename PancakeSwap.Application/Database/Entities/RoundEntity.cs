using System;
using SqlSugar;

namespace PancakeSwap.Application.Database.Entities
{
    /// <summary>
    /// Represents a prediction round.
    /// </summary>
    [SugarTable("rounds")]
    public class Round
    {
        /// <summary>
        /// Epoch identifier.
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false, ColumnName = "epoch")]
        public long Epoch { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "start_time")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "lock_time")]
        public DateTime LockTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "close_time")]
        public DateTime CloseTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "lock_price", ColumnDataType = "decimal(18,8)")]
        public decimal LockPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "close_price", ColumnDataType = "decimal(18,8)")]
        public decimal ClosePrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "status")]
        public int Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "total_amount", ColumnDataType = "decimal(18,8)")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "bull_amount", ColumnDataType = "decimal(18,8)")]
        public decimal BullAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "bear_amount", ColumnDataType = "decimal(18,8)")]
        public decimal BearAmount { get; set; }
    }
}
