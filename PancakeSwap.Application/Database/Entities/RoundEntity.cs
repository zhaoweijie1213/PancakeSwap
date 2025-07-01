using System;
using SqlSugar;
using PancakeSwap.Application.Enums;

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
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false, ColumnName = "id")]
        public long Id { get; set; }

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
        /// 当前价格。
        /// </summary>
        [SugarColumn(ColumnName = "current_price", ColumnDataType = "decimal(18,8)")]
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 回合状态。
        /// </summary>
        [SugarColumn(ColumnName = "status")]
        public RoundStatus Status { get; set; }

        /// <summary>
        /// 本回合的下注总金额。
        /// </summary>
        [SugarColumn(ColumnName = "total_amount", ColumnDataType = "decimal(18,8)")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 押涨总金额。
        /// </summary>
        [SugarColumn(ColumnName = "up_amount", ColumnDataType = "decimal(18,8)")]
        public decimal UpAmount { get; set; }

        /// <summary>
        /// 押跌总金额。
        /// </summary>
        [SugarColumn(ColumnName = "down_amount", ColumnDataType = "decimal(18,8)")]
        public decimal DownAmount { get; set; }

        /// <summary>
        /// 可分配奖金池。
        /// </summary>
        [SugarColumn(ColumnName = "reward_amount", ColumnDataType = "decimal(18,8)")]
        public decimal RewardAmount { get; set; }
    }
}
