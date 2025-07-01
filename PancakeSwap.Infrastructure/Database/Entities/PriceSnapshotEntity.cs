using System;
using SqlSugar;

namespace PancakeSwap.Infrastructure.Database.Entities
{
    /// <summary>
    ///     Price snapshot entity.
    /// </summary>
    [SugarTable("price_snapshots")]
    public class PriceSnapshotEntity
    {
        /// <summary>
        /// Primary key identifier.
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "id")]
        public long Id { get; set; }

        /// <summary>
        /// Snapshot timestamp.
        /// </summary>
        [SugarColumn(ColumnName = "timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Price value.
        /// </summary>
        [SugarColumn(ColumnName = "price", ColumnDataType = "decimal(18,8)")]
        public decimal Price { get; set; }
    }
}
