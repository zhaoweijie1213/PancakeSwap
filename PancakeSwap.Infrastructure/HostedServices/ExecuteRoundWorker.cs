using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.RPC.Eth.DTOs;
using PancakeSwap.Infrastructure.Blockchain.PancakePredictionV2.ContractDefinition;
using System;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

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

        private readonly Nethereum.Contracts.Contract? _mockOracle;
        private readonly Random _random = new();

        private readonly uint _intervalSeconds;    // 回合时长（300）
        private readonly TimeSpan _pollInterval;   // 每隔多久检查一次并调用
        private readonly uint _bufferSeconds;      // 可执行时间缓冲区

        private readonly IHostEnvironment _hostEnvironment;

        /// <summary>
        /// 初始化后台任务实例。
        /// </summary>
        /// <param name="configuration">应用配置。</param>
        /// <param name="logger">日志记录器。</param>
        /// <param name="hostEnvironment">宿主环境信息。</param>
        public ExecuteRoundWorker(IConfiguration configuration, ILogger<ExecuteRoundWorker> logger, IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
            // 读取环境变量
            var rpc = configuration["BSC_RPC"] ?? "http://127.0.0.1:8545";
            var pk = configuration["OPERATOR_PK"] ?? throw new("OPERATOR_PK 未配置");
            var contract = configuration["PREDICTION_ADDRESS"]
                           ?? configuration["CONTRACT_ADDR_LOCAL"]
                           ?? throw new("PREDICTION_ADDRESS 未配置");
            contract = contract.Trim();
            var mockOracle = configuration["MOCK_ORACLE_ADDR"];

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

            if (!string.IsNullOrWhiteSpace(mockOracle))
            {
                const string updateAbi = "[{\"inputs\":[{\"internalType\":\"int256\",\"name\":\"newPrice\",\"type\":\"int256\"}],\"name\":\"updateAnswer\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";
                _mockOracle = _web3Tx.Eth.GetContract(updateAbi, mockOracle);
            }

            // 读取 intervalSeconds 与 bufferSeconds（一次即可）
            _intervalSeconds = _contract.GetFunction("intervalSeconds").CallAsync<uint>().Result;
            _bufferSeconds = _contract.GetFunction("bufferSeconds").CallAsync<uint>().Result;

            // 建议  (interval + buffer) / 2 轮询一次，避免重复
            _pollInterval = TimeSpan.FromSeconds((_intervalSeconds + _bufferSeconds) / 2);
        }

        /// <summary>
        /// 后台任务主体，不断执行合约轮询与交易。
        /// </summary>
        /// <param name="stoppingToken">取消任务的标记。</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_hostEnvironment.IsDevelopment()) return;

            _logger.LogInformation("⏲️ ExecuteRoundWorker 已启动，当前网络 {Network}",
                _hostEnvironment.EnvironmentName);
            //准备常用 Function 句柄
            var pausedFn = _contract.GetFunction("paused");
            var pauseFn = _contract.GetFunction("pause");
            var unpauseFn = _contract.GetFunction("unpause");
            var executeFn = _contract.GetFunction("executeRound");
            var lockFn = _contract.GetFunction("genesisLockRound");
            var startFn = _contract.GetFunction("genesisStartRound");
            var startOnceFn = _contract.GetFunction("genesisStartOnce");
            var lockOnceFn = _contract.GetFunction("genesisLockOnce");

            // ---------- ⛑️ 自愈：若创世未完成则自动复位 ----------
            await EnsureGenesisAsync(pausedFn, pauseFn, unpauseFn, startFn, lockFn, startOnceFn, lockOnceFn, stoppingToken);
            await ExecuteRoundsAsync(executeFn, stoppingToken);
        }

        /// <summary>
        /// 在本地 Hardhat 网络上通过增加区块时间加速等待，
        /// 其他环境则按给定秒数延时。
        /// </summary>
        /// <param name="seconds">需要等待的秒数。</param>
        /// <param name="ct">取消标记。</param>
        /// <returns>异步任务。</returns>
        private async Task SleepOrFastForward(uint seconds, CancellationToken ct)
        {
            var chainId = (await _web3Tx.Net.Version.SendRequestAsync()).ToString();
            if (chainId == "31337")           // Hardhat 本地链
            {
                await _web3Tx.Client.SendRequestAsync<string>("evm_increaseTime", null, seconds);
                await _web3Tx.Client.SendRequestAsync<string>("evm_mine");
            }
            else
            {
                await Task.Delay(TimeSpan.FromSeconds(seconds), ct);
            }
        }

        /// <summary>
        /// 随机调整价格并推送到本地预言机，
        /// 仅在配置 <c>MOCK_ORACLE_ADDR</c> 时执行。
        /// </summary>
        /// <param name="ct">取消标记。</param>
        private async Task UpdateMockPriceAsync(CancellationToken ct)
        {
            if (_mockOracle == null) return;

            var updateFn = _mockOracle.GetFunction("updateAnswer");
            const decimal basePrice = 300m;
            var delta = basePrice * ((decimal)_random.NextDouble() * 0.02m - 0.01m);
            var priceValue = new BigInteger((long)Math.Floor((basePrice + delta) * 1_00000000m));

            HexBigInteger gas;
            try
            {
                gas = await updateFn.EstimateGasAsync(
                    _web3Tx.TransactionManager.Account.Address,
                    gas: null,
                    value: null,
                    functionInput: priceValue);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "预言机价格更新估算 gas 失败，使用默认值");
                gas = new HexBigInteger(150_000);
            }

            await updateFn.SendTransactionAndWaitForReceiptAsync(
                _web3Tx.TransactionManager.Account.Address,
                gas: gas,
                value: new HexBigInteger(BigInteger.Zero),
                functionInput: priceValue);
        }

        /// <summary>
        /// 计算距离下一次 executeRound 可以调用的秒数。
        /// </summary>
        /// <param name="epochFn">currentEpoch 查询函数。</param>
        /// <param name="roundsFn">rounds 查询函数。</param>
        /// <returns>需要等待的秒数。</returns>
        private async Task<uint> CalculateWaitSecondsAsync(
            Nethereum.Contracts.Function epochFn,
            Nethereum.Contracts.Function roundsFn)
        {
            var epoch = await epochFn.CallAsync<BigInteger>();
            var round = await roundsFn
                .CallDeserializingToObjectAsync<RoundsOutputDTO>(epoch);
            var block = await _web3Tx.Eth.Blocks
                .GetBlockWithTransactionsByNumber
                .SendRequestAsync(BlockParameter.CreateLatest());
            var now = (ulong)block.Timestamp.Value;
            var target = (ulong)round.LockTimestamp + _bufferSeconds / 2;
            return target <= now ? 1u : (uint)(target - now);
        }

        /// <summary>
        /// 创世流程与自愈逻辑。
        /// </summary>
        /// <param name="pausedFn">合约 paused 状态查询函数。</param>
        /// <param name="pauseFn">暂停函数。</param>
        /// <param name="unpauseFn">解除暂停函数。</param>
        /// <param name="startFn">genesisStartRound 函数。</param>
        /// <param name="lockFn">genesisLockRound 函数。</param>
        /// <param name="startOnceFn">genesisStartOnce 查询函数。</param>
        /// <param name="lockOnceFn">genesisLockOnce 查询函数。</param>
        /// <param name="token">取消标记。</param>
        private async Task EnsureGenesisAsync(
            Nethereum.Contracts.Function pausedFn,
            Nethereum.Contracts.Function pauseFn,
            Nethereum.Contracts.Function unpauseFn,
            Nethereum.Contracts.Function startFn,
            Nethereum.Contracts.Function lockFn,
            Nethereum.Contracts.Function startOnceFn,
            Nethereum.Contracts.Function lockOnceFn,
            CancellationToken token)
        {
            if (!await startOnceFn.CallAsync<bool>() || !await lockOnceFn.CallAsync<bool>())
            {
                if (!await pausedFn.CallAsync<bool>())
                {
                    _logger.LogWarning("检测到创世不完整，执行 pause() 复位…");
                    await pauseFn.SendTransactionAndWaitForReceiptAsync(
                        _web3Tx.TransactionManager.Account.Address,
                        gas: new HexBigInteger(350_000),
                        value: new HexBigInteger(BigInteger.Zero));

                    _logger.LogInformation("✅ 合约已暂停");
                }

                await unpauseFn.SendTransactionAndWaitForReceiptAsync(
                    _web3Tx.TransactionManager.Account.Address,
                    gas: new HexBigInteger(350_000),
                    value: new HexBigInteger(BigInteger.Zero));

                _logger.LogInformation("✅ 合约已解除暂停，genesis 标志已复位");
            }

            if (!await startOnceFn.CallAsync<bool>())
            {
                try
                {
                    var gas = await startFn.EstimateGasAsync();
                    var rc = await startFn.SendTransactionAndWaitForReceiptAsync(
                        _web3Tx.TransactionManager.Account.Address,
                        gas: gas,
                        value: new HexBigInteger(BigInteger.Zero));

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
                var delay = _intervalSeconds + _bufferSeconds / 2;
                _logger.LogInformation("等待 {Delay}s 后调用 genesisLockRound", delay);
                await Task.Delay(TimeSpan.FromSeconds(delay), token);

                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        await UpdateMockPriceAsync(token);
                        var gas = await lockFn.EstimateGasAsync();
                        var rc = await lockFn.SendTransactionAndWaitForReceiptAsync(
                            _web3Tx.TransactionManager.Account.Address,
                            gas: gas,
                            value: new HexBigInteger(BigInteger.Zero));

                        _logger.LogInformation("✅ 已调用 genesisLockRound，交易 {Hash} | 区块 {Block}",
                            rc.TransactionHash, rc.BlockNumber.Value);
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "genesisLockRound 调用失败，{Retry}s 后重试", _bufferSeconds / 2);
                        await Task.Delay(TimeSpan.FromSeconds(_bufferSeconds / 2), token);
                    }
                }

                // 新回合已开始，等待至可执行时间
                await SleepOrFastForward(_intervalSeconds + _bufferSeconds / 2, token);
            }
        }

        /// <summary>
        /// 周期性执行 executeRound。
        /// </summary>
        /// <param name="executeFn">executeRound 函数。</param>
        /// <param name="token">取消标记。</param>
        private async Task ExecuteRoundsAsync(Nethereum.Contracts.Function executeFn, CancellationToken token)
        {
            var currentEpochFn = _contract.GetFunction("currentEpoch");
            var roundsFn = _contract.GetFunction("rounds");

            while (!token.IsCancellationRequested)
            {
                try
                {
                    await UpdateMockPriceAsync(token);
                    var gas = await executeFn.EstimateGasAsync();
                    var rc = await executeFn.SendTransactionAndWaitForReceiptAsync(
                        _web3Tx.TransactionManager.Account.Address,
                        gas: gas,
                        value: new HexBigInteger(BigInteger.Zero));

                    _logger.LogInformation("✅ executeRound 交易成功：{Hash} | 区块 {Block}",
                        rc.TransactionHash, rc.BlockNumber.Value);
                }
                catch (SmartContractRevertException ex) when (ex.RevertMessage.Contains("Too early"))
                {
                    _logger.LogWarning("⌛ 太早，15 秒后重试");
                    await SleepOrFastForward(15, token);
                }
                catch (SmartContractRevertException ex) when (ex.RevertMessage.Contains("roundId must be larger than oracleLatestRoundId"))
                {
                    _logger.LogWarning("📈 预言机数据未更新，30 秒后重试");
                    await UpdateMockPriceAsync(token);
                    await SleepOrFastForward(30, token);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "executeRound 调用失败");
                }

                uint delay;
                try
                {
                    delay = await CalculateWaitSecondsAsync(currentEpochFn, roundsFn);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "⏱ 计算等待时间失败，使用默认间隔 {Poll}s", _pollInterval.TotalSeconds);
                    delay = (uint)_pollInterval.TotalSeconds;
                }

                try
                {
                    await SleepOrFastForward(delay, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }


    }
}
