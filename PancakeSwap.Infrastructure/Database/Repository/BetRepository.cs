using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PancakeSwap.Application.Database.Config;
using PancakeSwap.Application.Database.Entities;
using PancakeSwap.Application.Database.Repository;
using QYQ.Base.SqlSugar;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PancakeSwap.Infrastructure.Database.Repository
{
    /// <summary>
    /// 下注仓库实现
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="connectionString"></param>
    /// <param name="dbType"></param>
    public class BetRepository(ILogger<BetRepository> logger, IOptionsMonitor<DatabaseConfig> options, DbType dbType = DbType.Sqlite) : BaseRepository<BetEntity>(logger, options.CurrentValue.Default, dbType), IBetRepository
    {

    }
}
