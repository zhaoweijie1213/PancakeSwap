using System;
using System.Collections.Generic;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Web3;

namespace PancakeSwap.Infrastructure.Blockchain
{
    public abstract class ContractWeb3ServiceBase
    {
        protected ContractHandler ContractHandler { get; }

        protected ContractWeb3ServiceBase(IWeb3 web3, string contractAddress)
        {
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public virtual List<Type> GetAllFunctionTypes() => new();
        public virtual List<Type> GetAllEventTypes() => new();
        public virtual List<Type> GetAllErrorTypes() => new();
    }
}
