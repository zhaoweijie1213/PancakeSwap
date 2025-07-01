using System;
using System.Collections.Generic;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Web3;

namespace PancakeSwap.Infrastructure.Blockchain
{
    /// <summary>
    ///     Web3 合约服务基类，封装 <see cref="ContractHandler"/> 的创建逻辑。
    ///     派生类通过该处理器与链上合约进行交互。
    /// </summary>
    public abstract class ContractWeb3ServiceBase
    {
        /// <summary>
        ///     合约处理器，可发送交易或查询数据。
        /// </summary>
        protected ContractHandler ContractHandler { get; }

        /// <summary>
        ///     初始化基类并创建合约处理器。
        /// </summary>
        /// <param name="web3">Web3 实例</param>
        /// <param name="contractAddress">合约地址</param>
        protected ContractWeb3ServiceBase(IWeb3 web3, string contractAddress)
        {
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        /// <summary>
        ///     获取所有函数类型，便于反射处理。
        /// </summary>
        public virtual List<Type> GetAllFunctionTypes() => new();

        /// <summary>
        ///     获取所有事件 DTO 类型。
        /// </summary>
        public virtual List<Type> GetAllEventTypes() => new();

        /// <summary>
        ///     获取所有错误类型。
        /// </summary>
        public virtual List<Type> GetAllErrorTypes() => new();
    }
}
