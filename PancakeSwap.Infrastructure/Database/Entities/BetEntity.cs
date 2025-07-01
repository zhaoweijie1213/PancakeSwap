using SqlSugar;
using PancakeSwap.Application.Enums;

namespace PancakeSwap.Infrastructure.Database.Entities
{
    /// <summary>
    ///     Represents a bet within a round.
    /// </summary>
    [SugarTable("bets")]
    public class BetEntity
    {
        /// <summary>
        /// Primary key identifier.
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "id")]
        public long Id { get; set; }

        /// <summary>
        /// Related round identifier.
        /// </summary>
        [SugarColumn(ColumnName = "epoch_id")]
        public long EpochId { get; set; }

        /// <summary>
        /// User address.
        /// </summary>
        [SugarColumn(ColumnName = "user_address")]
        public string UserAddress { get; set; } = string.Empty;

        /// <summary>
        /// Bet amount.
        /// </summary>
        [SugarColumn(ColumnName = "amount", ColumnDataType = "decimal(18,8)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Bet position.
        /// </summary>
        [SugarColumn(ColumnName = "position")]
        public Position Position { get; set; }

        /// <summary>
        /// Whether the reward has been claimed.
        /// </summary>
        [SugarColumn(ColumnName = "claimed")]
        public bool Claimed { get; set; }

        /// <summary>
        /// Reward amount.
        /// </summary>
        [SugarColumn(ColumnName = "reward", ColumnDataType = "decimal(18,8)")]
        public decimal Reward { get; set; }
    }
}
