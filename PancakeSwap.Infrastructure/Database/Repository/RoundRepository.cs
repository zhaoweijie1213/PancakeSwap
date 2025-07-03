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
    /// 回合仓库实现
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="connectionString"></param>
    /// <param name="dbType"></param>
    public class RoundRepository(ILogger<RoundRepository> logger, IOptionsMonitor<DatabaseConfig> options, DbType dbType = DbType.MySql) : BaseRepository<RoundEntity>(logger, options.CurrentValue.Default, dbType), IRoundRepository
    {

    }
}
