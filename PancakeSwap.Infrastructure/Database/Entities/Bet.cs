using SqlSugar;

namespace PancakeSwap.Infrastructure.Database.Entities
{
    /// <summary>
    ///     Represents a bet within a round.
    /// </summary>
    [SugarTable("bets")]
    public class Bet
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "id")]
        public int Id { get; set; }

        [SugarColumn(ColumnName = "epoch")]
        public long Epoch { get; set; }

        [SugarColumn(ColumnName = "address")]
        public string Address { get; set; } = string.Empty;

        [SugarColumn(ColumnName = "amount", ColumnDataType = "decimal(18,8)")]
        public decimal Amount { get; set; }

        [SugarColumn(ColumnName = "position")]
        public int Position { get; set; }
    }
}
