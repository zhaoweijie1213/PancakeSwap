using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using PancakeSwap.Application.Enums;
using PancakeSwap.Application.Services;
using PancakeSwap.Infrastructure.Database;
using PancakeSwap.Application.Database.Entities;
using PancakeSwap.Infrastructure.Database.Migrations;
using PancakeSwap.Infrastructure.Services;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;


namespace PancakeSwap.Test;

public class RoundServiceTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly IServiceProvider _serviceProvider = factory.Services;

    [Fact]
    public async Task Should_Throw_When_NoPrice()
    {
        var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Db.Insertable(new RoundEntity
        {
            Id = 1,
            LockPrice = 10,
            Status = RoundStatus.Locked,
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
        var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Db.Insertable(new RoundEntity
        {
            Id = 1,
            LockPrice = 10,
            Status = RoundStatus.Locked,
            StartTime = DateTime.UtcNow,
            LockTime = DateTime.UtcNow,
            CloseTime = DateTime.UtcNow
        }).ExecuteCommandAsync();

        var mock = new Mock<IPriceFeed>();
        mock.Setup(m => m.GetLatestPriceAsync(It.IsAny<CancellationToken>())).ReturnsAsync(20m);

        var service = new RoundService(context, mock.Object);

        await service.SettleRoundAsync(1, CancellationToken.None);

        var round = await context.Db.Queryable<RoundEntity>().Where(r => r.Id == 1).FirstAsync();
        Assert.Equal(RoundStatus.Ended, round.Status);
    }
}
