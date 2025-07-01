using Microsoft.Extensions.Configuration;
using SqlSugar;
using PancakeSwap.Infrastructure.Database.Entities;

namespace PancakeSwap.Infrastructure.Database
{
    /// <summary>
    ///     SqlSugar database context.
    /// </summary>
    public class ApplicationDbContext
    {
        /// <summary>
        ///     Gets the SqlSugar client instance.
        /// </summary>
        public ISqlSugarClient Db { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        /// <param name="configuration">Configuration to fetch connection string.</param>
        public ApplicationDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Default");
            Db = new SqlSugarScope(new ConnectionConfig
            {
                ConnectionString = connectionString,
                DbType = DbType.PostgreSQL,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            });
        }
    }
}
