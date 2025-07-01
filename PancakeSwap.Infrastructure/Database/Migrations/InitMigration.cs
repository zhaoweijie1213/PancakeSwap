using SqlSugar;
using PancakeSwap.Infrastructure.Database.Entities;

namespace PancakeSwap.Infrastructure.Database.Migrations
{
    /// <summary>
    ///     Initial database migration.
    /// </summary>
    public static class InitMigration
    {
        /// <summary>
        ///     Executes the migration to create required tables.
        /// </summary>
        /// <param name="db">SqlSugar client.</param>
        public static void Run(ISqlSugarClient db)
        {
            db.CodeFirst.InitTables<RoundEntity, BetEntity>();
        }
    }
}
