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
- `BSC_TESTNET_RPC`：BSC 测试网 RPC 地址
- `PRIVATE_KEY`：部署或测试用私钥

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
在配置好环境变量后，执行 `npx hardhat run --network bsctest scripts/deploy.ts` 部署到 BSC 测试网。
