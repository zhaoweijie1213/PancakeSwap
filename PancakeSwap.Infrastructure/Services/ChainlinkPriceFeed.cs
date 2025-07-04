using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Nethereum.Web3;
using PancakeSwap.Application.Services;

namespace PancakeSwap.Infrastructure.Services
{
    /// <summary>
    /// 基于 Chainlink 的价格源实现。
    /// </summary>
    public class ChainlinkPriceFeed : IPriceFeed
    {
        /// <summary>
        /// 最新价格查询的 ABI 定义。
        /// </summary>
        private const string LatestAnswerAbi =
            "[{\"inputs\":[],\"name\":\"latestAnswer\",\"outputs\":[{\"internalType\":\"int256\",\"name\":\"\",\"type\":\"int256\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";

        private readonly IWeb3 _web3;

        /// <summary>
        /// 
        /// </summary>
        private readonly string _oracleAddress;

        /// <summary>
        /// 初始化实例。
        /// </summary>
        public ChainlinkPriceFeed(IWeb3 web3, IConfiguration configuration)
        {
            _web3 = web3;
            _oracleAddress =
                configuration.GetValue<string>("CHAINLINK_ORACLE") ??
                configuration.GetValue<string>("MOCK_ORACLE_ADDR") ??
                configuration.GetValue<string>("ORACLE_TEST") ??
                configuration.GetValue<string>("ORACLE_MAIN") ??
                string.Empty;
        }

        /// <summary>
        /// 获取最新价格。
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<decimal?> GetLatestPriceAsync(CancellationToken ct)
        {
            if (string.IsNullOrEmpty(_oracleAddress))
            {
                return null;
            }

            var contract = _web3.Eth.GetContract(LatestAnswerAbi, _oracleAddress);
            var function = contract.GetFunction("latestAnswer");
            var value = await function.CallAsync<BigInteger>();
            return (decimal)value / 1_00000000m;
        }
    }
}
