using Microsoft.Extensions.Logging;
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
    public class RoundRepository(ILogger<RoundRepository> logger, string connectionString, DbType dbType = DbType.Sqlite) : BaseRepository<RoundEntity>(logger, connectionString, dbType), IRoundRepository
    {

    }
}
