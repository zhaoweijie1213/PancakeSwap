# PancakeSwap Prediction Backend

## 项目简介
该项目提供 PancakeSwap Prediction 的链下支撑服务，并附带 Solidity 合约示例及 Hardhat 配置。

## 快速开始
```bash
# 安装 .NET 依赖
./dotnet-install.sh
DOTNET_ROOT=$HOME/.dotnet dotnet tool install --global dotnet-format
DOTNET_ROOT=$HOME/.dotnet dotnet restore

# 安装 Node 依赖
npm install
```

环境变量：
- `LOCAL_RPC`：本地 Hardhat RPC 地址
- `LOCAL_MNEMONIC`：本地链助记词
- `LOCAL_PK`：本地链私钥
- `BSC_TESTNET_RPC`：BSC 测试网 RPC 地址
- `PRIVATE_KEY`：部署或测试用私钥
- `ORACLE_TEST`：测试网预言机合约地址
- `ORACLE_MAIN`：主网预言机合约地址
- `ADMIN`：合约超级管理员地址
- `OPERATOR`：负责执行的运营者地址
- `BSC_RPC`：后台监听事件的 BSC RPC 地址
- `OPERATOR_PK`：运营者私钥
- `CONTRACT_ADDR_LOCAL`：本地部署的合约地址
- `MOCK_ORACLE_ADDR`：本地 Mock 预言机地址
- `INTERVAL_SECONDS`：回合时间间隔
- `BUFFER_SECONDS`：执行缓冲区时间
- `PG_CONNECTION`：PostgreSQL 连接字符串

## 架构概览
- `PancakeSwap.Api`：HTTP 接口与 SignalR 推送
- `PancakeSwap.Application`：业务逻辑
- `PancakeSwap.Infrastructure`：链上与数据库适配
- `contracts`：Solidity 合约与 Hardhat 配置

## 常见命令
```bash
# 代码格式检查
DOTNET_ROOT=$HOME/.dotnet dotnet format --verify-no-changes
# 构建与测试
DOTNET_ROOT=$HOME/.dotnet dotnet build -c Release
DOTNET_ROOT=$HOME/.dotnet dotnet test
# Solidity 编译
npx hardhat compile
```

## 部署说明
### bsctest测试网测试

在配置好环境变量后，执行 `npx hardhat run --network bsctest scripts/deploy.ts` 部署到 BSC 测试网。

### localhost测试

**运行本地链**

```bash
npm run node  # 终端 A：本地链
```

**部署MockAggregator代替Chainlink 预言机**

```bash
npm run deploy:local:mock  # 终端 B：MockAggregator
```

**部署智能合约**

```bash
npm run deploy:local
```

**自动开奖**

1. 前端

```bash
npm run keeper:local    # 终端 C：自动开奖
```

2. 后端使用ExecuteRoundWorker自动执行创世区块和每轮开奖
