using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using System;
using System.Numerics;
using Nethereum.Web3.Accounts;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace PancakeSwap.Infrastructure.Services
{
    /// <summary>
    /// 定时调用 PancakePrediction.executeRound() 的后台任务。
    /// 本地链 & Testnet 调试均可用；生产请考虑 Chainlink Automation。
    /// </summary>
    public class ExecuteRoundWorker : BackgroundService
    {
        private readonly ILogger<ExecuteRoundWorker> _logger;
        private readonly IWeb3 _web3Tx;            // 带签名账户，用于发交易
        private readonly Nethereum.Contracts.Contract _contract;

        private readonly uint _intervalSeconds;    // 回合时长（300）
        private readonly TimeSpan _pollInterval;   // 每隔多久检查一次并调用
        private readonly string _networkName;
        private readonly bool _isDevelopment;

        public ExecuteRoundWorker(IConfiguration cfg, ILogger<ExecuteRoundWorker> logger)
        {
            _logger = logger;

            // 读取环境变量
            var rpc = cfg["BSC_RPC"] ?? "http://127.0.0.1:8545";
            var pk = cfg["OPERATOR_PK"] ?? throw new("OPERATOR_PK 未配置");
            var contract = cfg["PREDICTION_ADDRESS"] ?? throw new("PREDICTION_ADDRESS 未配置");
            _networkName = cfg["ASPNETCORE_ENVIRONMENT"] ?? "Development";
            _isDevelopment = string.Equals(_networkName, "Development", StringComparison.OrdinalIgnoreCase);

            // 创建签名账号
            var account = new Account(pk);
            _web3Tx = new Web3(account, rpc);

            // 连接合约（只需要 ABI）
            var asm = typeof(ExecuteRoundWorker).Assembly;
            var resource = "PancakeSwap.Infrastructure.Blockchain.PancakePredictionV2.abi";
            using var stream = asm.GetManifestResourceStream(resource)
                ?? throw new InvalidOperationException("ABI resource not found");
            using var reader = new StreamReader(stream);
            var abi = reader.ReadToEnd();
            _contract = _web3Tx.Eth.GetContract(abi, contract);

            // 读取 intervalSeconds (一次即可)
            _intervalSeconds = _contract.GetFunction("intervalSeconds").CallAsync<uint>().Result;

            // 建议  (interval + buffer) / 2 轮询一次，避免重复
            _pollInterval = TimeSpan.FromSeconds(_intervalSeconds / 2);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("⏲️ ExecuteRoundWorker started on {Network}, interval {Poll}s",
                _networkName, _pollInterval.TotalSeconds);

            var executeFn = _contract.GetFunction("executeRound");
            var lockFn = _contract.GetFunction("genesisLockRound");   // 仅本地链调试可用
            var startFn = _contract.GetFunction("genesisStartRound");
            var startOnceFn = _contract.GetFunction("genesisStartOnce");
            var lockOnceFn = _contract.GetFunction("genesisLockOnce");

            if (_isDevelopment)
            {
                // 本地环境首次启动时需要调用 genesisStartRound 与 genesisLockRound
                if (!await startOnceFn.CallAsync<bool>())
                {
                    try
                    {
                        var gas = await startFn.EstimateGasAsync().ConfigureAwait(false);
                        var receipt = await startFn.SendTransactionAndWaitForReceiptAsync(
                            from: _web3Tx.TransactionManager.Account.Address,
                            gas: gas,
                            value: new HexBigInteger(BigInteger.Zero));

                        _logger.LogInformation("✅ genesisStartRound tx {Hash} | block {Block}",
                            receipt.TransactionHash, receipt.BlockNumber.Value);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "genesisStartRound failed");
                    }
                }

                if (!await lockOnceFn.CallAsync<bool>())
                {
                    try
                    {
                        var gas = await lockFn.EstimateGasAsync().ConfigureAwait(false);
                        var receipt = await lockFn.SendTransactionAndWaitForReceiptAsync(
                            from: _web3Tx.TransactionManager.Account.Address,
                            gas: gas,
                            value: new HexBigInteger(BigInteger.Zero));

                        _logger.LogInformation("✅ genesisLockRound tx {Hash} | block {Block}",
                            receipt.TransactionHash, receipt.BlockNumber.Value);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "genesisLockRound failed");
                    }
                }
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // 1️⃣ 发送交易
                    var gas = await executeFn.EstimateGasAsync().ConfigureAwait(false);
                    var tx = await executeFn.SendTransactionAndWaitForReceiptAsync(
                                    from: _web3Tx.TransactionManager.Account.Address,
                                    gas: gas,
                                    value: new HexBigInteger(BigInteger.Zero));

                    _logger.LogInformation("✅ executeRound tx {Hash} | block {Block}",
                        tx.TransactionHash, tx.BlockNumber.Value);

                }
                catch (Exception ex)
                {
                    // 常见 revert: Too early, Round not bettable, Not operator …
                    _logger.LogWarning(ex, "executeRound failed");
                }

                try
                {
                    await Task.Delay(_pollInterval, stoppingToken);
                }
                catch (OperationCanceledException) { break; }
            }
        }
    }
}
