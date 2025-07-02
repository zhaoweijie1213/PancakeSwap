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

namespace PancakeSwap.Infrastructure.HostedServices
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
        private readonly uint _bufferSeconds;      // 可执行时间缓冲区

        private readonly IHostEnvironment _hostEnvironment;

        public ExecuteRoundWorker(IConfiguration configuration, ILogger<ExecuteRoundWorker> logger, IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
            // 读取环境变量
            var rpc = configuration["BSC_RPC"] ?? "http://127.0.0.1:8545";
            var pk = configuration["OPERATOR_PK"] ?? throw new("OPERATOR_PK 未配置");
            var contract = configuration["PREDICTION_ADDRESS"] ?? throw new("PREDICTION_ADDRESS 未配置");

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

            // 读取 intervalSeconds 与 bufferSeconds（一次即可）
            _intervalSeconds = _contract.GetFunction("intervalSeconds").CallAsync<uint>().Result;
            _bufferSeconds = _contract.GetFunction("bufferSeconds").CallAsync<uint>().Result;

            // 建议  (interval + buffer) / 2 轮询一次，避免重复
            _pollInterval = TimeSpan.FromSeconds((_intervalSeconds + _bufferSeconds) / 2);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("⏲️ ExecuteRoundWorker 已启动，当前网络 {Network}，轮询间隔 {Poll}s",
                _hostEnvironment.EnvironmentName, _pollInterval.TotalSeconds);

            var executeFn = _contract.GetFunction("executeRound");
            var lockFn = _contract.GetFunction("genesisLockRound");
            var startFn = _contract.GetFunction("genesisStartRound");
            var startOnceFn = _contract.GetFunction("genesisStartOnce");
            var lockOnceFn = _contract.GetFunction("genesisLockOnce");

            if (_hostEnvironment.IsDevelopment())
            {
                // ------ 创世两步，仅本地调试需要 ------
                if (!await startOnceFn.CallAsync<bool>())
                {
                    try
                    {
                        var gas = await startFn.EstimateGasAsync();
                        var rc = await startFn.SendTransactionAndWaitForReceiptAsync(
                                    _web3Tx.TransactionManager.Account.Address, gas, new HexBigInteger(BigInteger.Zero));

                        _logger.LogInformation("✅ 已调用 genesisStartRound，交易 {Hash} | 区块 {Block}",
                            rc.TransactionHash, rc.BlockNumber.Value);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "genesisStartRound 调用失败");
                    }
                }

                if (!await lockOnceFn.CallAsync<bool>())
                {
                    try
                    {
                        _logger.LogInformation("等待 {Delay}s 后调用 genesisLockRound", _intervalSeconds);
                        await Task.Delay(TimeSpan.FromSeconds(_intervalSeconds), stoppingToken);

                        var gas = await lockFn.EstimateGasAsync();
                        var rc = await lockFn.SendTransactionAndWaitForReceiptAsync(
                                    _web3Tx.TransactionManager.Account.Address, gas, new HexBigInteger(BigInteger.Zero));

                        _logger.LogInformation("✅ 已调用 genesisLockRound，交易 {Hash} | 区块 {Block}",
                            rc.TransactionHash, rc.BlockNumber.Value);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "genesisLockRound 调用失败");
                    }
                }
            }

            // ------ 周期性 executeRound ------
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var gas = await executeFn.EstimateGasAsync();
                    var rc = await executeFn.SendTransactionAndWaitForReceiptAsync(
                                _web3Tx.TransactionManager.Account.Address, gas, new HexBigInteger(BigInteger.Zero));

                    _logger.LogInformation("✅ executeRound 交易成功：{Hash} | 区块 {Block}",
                        rc.TransactionHash, rc.BlockNumber.Value);
                }
                catch (Exception ex)
                {
                    // 常见 revert: Too early / Not operator / Round not bettable…
                    _logger.LogWarning(ex, "executeRound 调用失败");
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
