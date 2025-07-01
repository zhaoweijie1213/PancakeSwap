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
    /// 下注仓库实现
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="connectionString"></param>
    /// <param name="dbType"></param>
    public class BetRepository(ILogger<BetRepository> logger, string connectionString, DbType dbType = DbType.Sqlite) : BaseRepository<BetEntity>(logger, connectionString, dbType), IBetRepository
    {

    }
}
