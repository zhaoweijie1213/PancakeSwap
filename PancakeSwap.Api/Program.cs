using PancakeSwap.Application.Database.Config;
using PancakeSwap.Infrastructure.Database;
using PancakeSwap.Infrastructure.Database.Migrations;
using PancakeSwap.Application.Services;
using PancakeSwap.Infrastructure.Services;
using PancakeSwap.Api.HostedServices;
using PancakeSwap.Api.Hubs;
using Nethereum.Web3;
using QYQ.Base.Common.IOCExtensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddQYQSerilog();
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddMultipleService("^PancakeSwap");
builder.Services.AddSingleton<ApplicationDbContext>();
builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection("ConnectionStrings:Default"));

var rpc = builder.Configuration.GetValue<string>("BSC_RPC");
builder.Services.AddSingleton<IWeb3>(_ => new Web3(rpc));
builder.Services.AddHostedService<ChainEventListener>();
builder.Services.AddHostedService<RoundBroadcastService>();
builder.Services.AddHostedService<ChartBroadcastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();
app.MapHub<PredictionHub>("/predictionHub");

var dbContext = app.Services.GetRequiredService<ApplicationDbContext>();
InitMigration.Run(dbContext.Db);

app.Run();
