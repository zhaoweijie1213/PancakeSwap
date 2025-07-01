using PancakeSwap.Infrastructure.Database;
using PancakeSwap.Infrastructure.Database.Migrations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<PancakeSwap.Infrastructure.Database.ApplicationDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

var dbContext = app.Services.GetRequiredService<ApplicationDbContext>();
InitMigration.Run(dbContext.Db);

app.Run();
