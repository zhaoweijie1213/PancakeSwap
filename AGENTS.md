# PancakeSwap Prediction Backend

## 0. 背景

你是 .NET 后端工程师，需要实现 PancakeSwap Prediction 的链下支撑服务：

1. **Presentation**：`PancakeSwap.Api`
   - ASP.NET Core 8 Minimal API + SignalR
   - 注册 `IHostedService`（BackgroundService）用来监听 BSC 链上事件并持久化
2. **Application**：`PancakeSwap.Application`
   - 纯 C# 业务用例 / Service / 接口定义
3. **Infrastructure**：`PancakeSwap.Infrastructure`
   - Nethereum、SqlSugar（`QYQ.Base.SqlSugar`）、Redis —— 具体实现 Application 接口
   - 提供监听逻辑所需的链上与数据库适配

**注意**：仓库不包含前端代码，/frontend 目录若存在仅作示例演示；禁止 Codex 修改。

### 0.1 SqlSugar 使用示例（仅作说明，禁止直接复制到业务代码）

```c#
using ClubHub.Models.Config;
using QYQ.Base.SqlSugar;
...
public class ExampleRepository(ILogger<ExampleRepository> logger, IOptionsMonitor<DatabaseConfig> options)
    : BaseRepository<ExampleEntity>(logger, options.CurrentValue.Log)
{
    public async Task<ExampleEntity> FindAsync(string id)
    {
        return await Db.Queryable<ExampleEntity>()
                       .Where(e => e.Id == id)
                       .FirstAsync();
    }
}
```

## 1. 代码规范

### 1.1 .NET

- 目标框架：`net9.0`
- 必须通过 `dotnet format --verify-no-changes`（已在 pre-commit 钩子）
- 公共 API 须带 XML 注释；Domain 层禁止直接引用 **SqlSugar**，仅能通过 Application 层定义的接口访问持久化逻辑
- 不得把私钥、Token 写进源码，使用 `<ENV_*>` 占位
- 注释使用简体中文

### 1.2 通用命名

- 类、接口：PascalCase；私有字段 `_camelCase`
- 文件名 = 顶级类名
- 异步方法后缀 `Async`

## 2. 构建与依赖

```bash
dotnet restore
# 安装 ORM
dotnet add package QYQ.Base.SqlSugar
docker compose up -d db redis bsc-node
```

## 3. 代码质量检查（必须全部通过）

先检测是否所有方法和属性都有注释,如果没有则补充完善,然后进行质量检查(注释必须描述属性的作用、方法的功能，保证初学者可以看懂,不能敷衍)

```bash
dotnet format --verify-no-changes
dotnet build -c Release
dotnet test
```

## 4. PR 规范

- 分支：`feat/<模块>` | `fix/<问题>` | `chore/<任务>`
- PR 描述需包含：
  1. 变更摘要
  2. 关联 Issue/Task ID
  3. 测试结果或链上 Tx 链接
- Push 后触发 `.github/workflows/ci-dotnet.yml`

## 5. README.md 编写规范

- **语言**：必须使用 **简体中文**。
- **时间格式**：如需写日期／时间，一律采用 **北京时间（UTC + 8）**，格式示例
  - 日期：`2025-07-01`
  - 日期时间：`2025-07-01 15:30 (UTC+8)`
- **推荐章节顺序**
  1. 项目简介
  2. 快速开始（本地启动脚本、必需环境变量）
  3. 架构概览（文字 + 图示）
  4. 常见命令（构建、测试、运行）
  5. 部署说明（Docker / Kubernetes）
- **保持同步**：当新增环境变量或启动脚本时，请同时更新 README。Codex 在自动修改相关文件后，也需自动修订 README 以保证一致性。

## 6. 禁改目录／文件

- `/frontend/**`
- `**/*-secret.json`
- `*.lock`（除非确有必要升级依赖）

> Codex：遇到不明确的需求，请在 PR 描述中提问，而不是自行猜测。