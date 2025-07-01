using System;
using SqlSugar;
using PancakeSwap.Application.Enums;

namespace PancakeSwap.Infrastructure.Database.Entities
{
    /// <summary>
    ///     Represents a prediction round.
    /// </summary>
    [SugarTable("rounds")]
    public class RoundEntity
    {
        /// <summary>
        ///     Round identifier.
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false, ColumnName = "id")]
        public long Id { get; set; }

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

        [SugarColumn(ColumnName = "current_price", ColumnDataType = "decimal(18,8)")]
        public decimal CurrentPrice { get; set; }

        [SugarColumn(ColumnName = "status")]
        public RoundStatus Status { get; set; }

        [SugarColumn(ColumnName = "total_amount", ColumnDataType = "decimal(18,8)")]
        public decimal TotalAmount { get; set; }

        [SugarColumn(ColumnName = "up_amount", ColumnDataType = "decimal(18,8)")]
        public decimal UpAmount { get; set; }

        [SugarColumn(ColumnName = "down_amount", ColumnDataType = "decimal(18,8)")]
        public decimal DownAmount { get; set; }

        [SugarColumn(ColumnName = "reward_amount", ColumnDataType = "decimal(18,8)")]
        public decimal RewardAmount { get; set; }
    }
}
