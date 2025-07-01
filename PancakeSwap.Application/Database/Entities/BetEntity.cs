using SqlSugar;

namespace PancakeSwap.Application.Database.Entities
{
    /// <summary>
    /// Represents a bet within a round.
    /// </summary>
    [SugarTable("bets")]
    public class Bet
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "id")]
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "epoch")]
        public long Epoch { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "address")]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "amount", ColumnDataType = "decimal(18,8)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "position")]
        public int Position { get; set; }
    }
}
