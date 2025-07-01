using SqlSugar;

namespace PancakeSwap.Application.Database.Entities
{
    /// <summary>
    /// 表示用户在某回合中的下注记录。
    /// </summary>
    [SugarTable("bet")]
    public class BetEntity
    {
        /// <summary>
        /// 主键自增编号。
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "id")]
        public int Id { get; set; }

        /// <summary>
        /// 所属回合编号。
        /// </summary>
        [SugarColumn(ColumnName = "epoch")]
        public long Epoch { get; set; }

        /// <summary>
        /// 用户地址。
        /// </summary>
        [SugarColumn(ColumnName = "address")]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// 下注金额。
        /// </summary>
        [SugarColumn(ColumnName = "amount", ColumnDataType = "decimal(18,8)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 下注方向。
        /// </summary>
        [SugarColumn(ColumnName = "position")]
        public int Position { get; set; }
    }
}
