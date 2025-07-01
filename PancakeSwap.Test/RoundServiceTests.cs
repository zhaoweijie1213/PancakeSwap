using Moq;
using PancakeSwap.Application.Enums;
using PancakeSwap.Application.Services;
using PancakeSwap.Infrastructure.Database;
using PancakeSwap.Infrastructure.Database.Migrations;
using PancakeSwap.Infrastructure.Services;
using SqlSugar;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using PancakeSwap.Infrastructure.Database.Entities;
using System;
using System.IO;


namespace PancakeSwap.Test;

public class RoundServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var dbPath = Path.GetTempFileName();
        var context = new ApplicationDbContext(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
        {
            {"ConnectionStrings:Default", $"DataSource={dbPath}"}
        }).Build());
        context.Db.DbMaintenance.CreateDatabase();
        InitMigration.Run(context.Db);
        return context;
    }

    [Fact]
    public async Task Should_Throw_When_NoPrice()
    {
        var context = CreateContext();
        await context.Db.Insertable(new RoundEntity
        {
            Epoch = 1,
            LockPrice = 10,
            Status = (int)RoundStatus.Locked,
            StartTime = DateTime.UtcNow,
            LockTime = DateTime.UtcNow,
            CloseTime = DateTime.UtcNow
        }).ExecuteCommandAsync();

        var mock = new Mock<IPriceFeed>();
        mock.Setup(m => m.GetLatestPriceAsync(It.IsAny<CancellationToken>())).ReturnsAsync((decimal?)null);

        var service = new RoundService(context, mock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.SettleRoundAsync(1, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Mark_Bull_Wins_When_ClosePrice_Higher()
    {
        var context = CreateContext();
        await context.Db.Insertable(new RoundEntity
        {
            Epoch = 1,
            LockPrice = 10,
            Status = (int)RoundStatus.Locked,
            StartTime = DateTime.UtcNow,
            LockTime = DateTime.UtcNow,
            CloseTime = DateTime.UtcNow
        }).ExecuteCommandAsync();

        var mock = new Mock<IPriceFeed>();
        mock.Setup(m => m.GetLatestPriceAsync(It.IsAny<CancellationToken>())).ReturnsAsync(20m);

        var service = new RoundService(context, mock.Object);

        await service.SettleRoundAsync(1, CancellationToken.None);

        var round = await context.Db.Queryable<RoundEntity>().Where(r => r.Epoch == 1).FirstAsync();
        Assert.Equal((int)BetPosition.Bull, round.WinningPosition);
    }
}
