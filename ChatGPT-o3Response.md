下面给你一份 **“小白到 MVP” 的实战路线图**——把 PancakeSwap Prediction 克隆拆成 **10 个渐进任务**，每个任务都附了可直接复制到 Copilot Chat / Codex CLI 的 **Prompt 模板**、你需要做的人工检查，以及继续前进的触发条件。跟着做即可逐步完成后端（.NET + Nethereum）+ 链上合约的最小可运行版本。整套流程假设：

- 你已把前文讨论好的 **AGENTS.md** 放在仓库根目录并 push。
- 电脑装好了 **.NET 8 SDK、Docker Desktop、Git、VS Code + GitHub Copilot**。
- 已准备一条 **BSC 测试网（或本地区块链模拟器）RPC**。

------

## 任务 0　准备工作（T0）

| 动作                                                         | 说明              |
| ------------------------------------------------------------ | ----------------- |
| 创建空仓库 `PancakeSwap`，把 `AGENTS.md` push 上去           | 让 Codex 读到规范 |
| `docker pull ghcr.io/openchain/bsc-dev`（或任何本地 BSC 节点镜像） | 本地链环境        |
| 在 VS Code 打开仓库，确保 Copilot Chat 就绪                  | 开始对话          |

触发条件：`git push` 后仓库里只有 `AGENTS.md`，GitHub Actions 通过。

------

## 任务 1　生成三层项目骨架（T0 + 15 min）

> **Prompt ①（选中仓库根目录）**
>
> ```
> 生成符合 AGENTS.md 的解决方案骨架：
> 1. PancakeSwap.sln
> 2. 项目 PancakeSwap.Api（ASP.NET Core 8 Minimal API）
> 3. 项目 PancakeSwap.Application
> 4. 项目 PancakeSwap.Infrastructure
> 5. 在 Api 的 Program.cs 注册一个空的 BackgroundService 叫 ChainEventListener
> ```

人工检查

```bash
dotnet restore
dotnet build -c Release
dotnet format --verify-no-changes
```

触发条件：构建及格式化全部通过。

------

## 任务 2　Fork 智能合约并编译（T0 + 30 min）

> **Prompt ②（新建 `contracts/` 文件夹后选中）**
>
> ```
> Fork PancakePredictionV2.sol，修改：
> - 回合时长 180 秒
> - treasuryFee 4%
> - 下注币种固定 BNB
> 生成 Solidity 文件并附 Hardhat 配置（BSC 测试网）
> ```

人工检查

1. 用 Remix 或 `hardhat test` 编译通过。
2. 合约变量是否按需求改好：`intervalSeconds=180`、`treasuryFee=400`（BP）。

触发条件：`npx hardhat test` 绿灯。

------

## 任务 3　生成 Nethereum Service（T0 + 45 min）

> **Prompt ③**
>
> ```
> 使用 Nethereum CodeGen 生成 Fork 后合约的 Service/DTO，放到
> PancakeSwap.Infrastructure/Blockchain/
> 并在 csproj 里引用 Nethereum.Web3
> ```

人工检查：`RoundDTO.cs`、`PredictionService.cs` 生成完毕且编译过。

触发条件：`dotnet build` 再次全绿。

------

## 任务 4　设计数据库 & 迁移（T0 + 1 h）

> **Prompt ④**
>
> ```
> 在 Infrastructure 新建实体 Round、Bet，
> Round 字段：Epoch(bigint PK)、StartTime、LockTime、CloseTime、
> LockPrice(decimal18,8)、ClosePrice(decimal18,8)、Status(int)、TotalAmount、BullAmount、BearAmount。
> Bet 字段：Id, Epoch, Address, Amount, Position(int)。
> 用 SqlSugar PostgreSQL 生成 DbContext 和首个迁移 Init。
> ```

人工检查

```bash
dotnet ef database update --project PancakeSwap.Infrastructure --startup-project PancakeSwap.Api
```

触发条件：数据库表成功生成。

------

## 任务 5　实现 RoundService 接口（T0 + 1 h 15 min）

> **Prompt ⑤**
>
> ```
> 在 Application 创建接口 IRoundService，方法：
> - Task<long> CreateNextRoundAsync(CancellationToken ct)
> - Task LockRoundAsync(long epoch, CancellationToken ct)
> - Task SettleRoundAsync(long epoch, CancellationToken ct)
> 在 Infrastructure 实现 RoundService，内部分别：
> 1. CreateNextRoundAsync：插入新 Round，Status=Pending
> 2. LockRoundAsync：调用 Nethereum 读取 Chainlink 价，更新 LockPrice
> 3. SettleRoundAsync：对比价格，高的方向胜，写 ClosePrice & Status
> ```
>
> Copilot 会补齐代码，你只需确认注入 DbContext、Web3。

触发条件：`dotnet test` 里的样例用例通过（Codex 会自动生成）。

------

## 任务 6　补完 ChainEventListener（T0 + 1 h 30 min）

> **Prompt ⑥（选中 ChainEventListener.cs）**
>
> ```
> 监听 BSC_RPC 上 PancakePrediction 合约 EndRound 事件，
> 每 3 秒轮询：若收到事件，解析 epoch & closePrice，
> 调用 RoundService.SettleRoundAsync(epoch)。
> 使用 ILogger 记录异常；在停止时取消订阅。
> ```

人工检查：运行 `dotnet run --project PancakeSwap.Api`，后台能打印事件日志（可先在本地链手动 emit）。

触发条件：事件处理无异常。

------

## 任务 7　暴露 API 与 SignalR（T0 + 2 h）

> **Prompt ⑦（选中 Api 项目 Program.cs）**
>
> ```
> 1. 注册 SignalR Hub /predictionHub，向客户端推送：
>    { epoch, secondsRemaining, bullAmount, bearAmount }
> 2. 创建 Minimal API:
>    POST /predictions/{epoch}/bet  -> 返回待签名交易数据
>    GET  /predictions/current      -> 返回当前回合详情
> ```

Codex 会生成 Hub、DTO 和路由。

人工检查：Swagger 页面能访问两个端点；SignalR JS 客户端连上后收到倒计时 JSON。

------

## 任务 8　单元测试 & CI 扩充（T0 + 2 h 30 min）

> **Prompt ⑧**
>
> ```
> 为 RoundService 编写 xUnit 测试：
> - Should_Throw_When_NoPrice
> - Should_Mark_Bull_Wins_When_ClosePrice_Higher
> 使用 Moq 或 FakeItEasy Mock IPriceFeed
> 更新 ci-dotnet.yml 加 `dotnet test --collect:"XPlat Code Coverage"`
> ```

运行 `dotnet test`，生成覆盖率报告。

触发条件：CI 绿灯且覆盖率 > 70%。

------

## 任务 9　同步文档（T0 + 3 h）

> **Prompt ⑨（选中 README.md 顶部）**
>
> ```
> 更新 README：
> - 在“快速开始”加入截至北京时间 2025-07-01 15:30 的新命令
> - 在“API 列表”加入 /predictions/current、/predictions/{epoch}/bet
> ```

人工检查：全文简体中文，时间格式 `YYYY-MM-DD HH:mm (UTC+8)`。

触发条件：README 与代码保持一致，语法无误。

------

## 任务 10　实战测试 & Demo（T0 + 4 h）

1. **启动全部服务**

   ```bash
   docker compose up -d    # PG / Redis / bsc-dev
   export BSC_RPC=http://localhost:8545
   dotnet run --project PancakeSwap.Api
   ```

2. **用 Hardhat 部署 Fork 合约到本地链，记录地址**

3. **调用 API 创建 -> 锁盘 -> 下注 -> 开奖**

4. **查看数据库 Round/Bet 表**，数据正确。

触发条件：整条链上 & 后端闭环跑通，一切 OK ⇒ MVP 完成！

------

### 后续循环

| 阶段           | 你做的事                       | 让 Codex 做的事                       |
| -------------- | ------------------------------ | ------------------------------------- |
| 挑功能写 Issue | 拆任务、写 Acceptance Criteria | 生成/修改代码、更新迁移、写测试       |
| 本地验证       | dotnet format/build/test       | ——                                    |
| 提 PR          | 人工 Review                    | 如果测试挂了，Codex 自动修复          |
| 部署           | Docker/K8s                     | 生成 Dockerfile、Helm、GitHub Actions |

保持 **Issue → Prompt → PR → CI** 的节奏，就能持续迭代出可上线的版本。

------

## 总结一句话

只要把上面 10 个任务按顺序推进，你就能从 **0 Web3 经验** 在 **半天左右**拿到一个本地完整跑通的 PancakeSwap Prediction 后端，并且每一步都让 Codex 自动产出大部分样板代码。后续再按同样节奏扩功能、做安全审计和性能优化即可。祝你开发顺利，遇到具体卡点随时再聊！