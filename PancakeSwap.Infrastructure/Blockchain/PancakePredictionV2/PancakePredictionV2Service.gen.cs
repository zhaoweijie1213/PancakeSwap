using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;
using PancakeSwap.Infrastructure.Blockchain.PancakePredictionV2.ContractDefinition;
using PancakeSwap.Infrastructure.Blockchain;

namespace PancakeSwap.Infrastructure.Blockchain.PancakePredictionV2
{
    public partial class PancakePredictionV2Service : PancakePredictionV2ServiceBase
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.IWeb3 web3, PancakePredictionV2Deployment pancakePredictionV2Deployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<PancakePredictionV2Deployment>().SendRequestAndWaitForReceiptAsync(pancakePredictionV2Deployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.IWeb3 web3, PancakePredictionV2Deployment pancakePredictionV2Deployment)
        {
            return web3.Eth.GetContractDeploymentHandler<PancakePredictionV2Deployment>().SendRequestAsync(pancakePredictionV2Deployment);
        }

        public static async Task<PancakePredictionV2Service> DeployContractAndGetServiceAsync(Nethereum.Web3.IWeb3 web3, PancakePredictionV2Deployment pancakePredictionV2Deployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, pancakePredictionV2Deployment, cancellationTokenSource);
            return new PancakePredictionV2Service(web3, receipt.ContractAddress);
        }

        public PancakePredictionV2Service(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress)
        {
        }

    }


    public partial class PancakePredictionV2ServiceBase : ContractWeb3ServiceBase
    {

        public PancakePredictionV2ServiceBase(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress)
        {
        }

        public Task<BigInteger> IntervalSecondsQueryAsync(IntervalSecondsFunction intervalSecondsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IntervalSecondsFunction, BigInteger>(intervalSecondsFunction, blockParameter);
        }


        public virtual Task<BigInteger> IntervalSecondsQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IntervalSecondsFunction, BigInteger>(null, blockParameter);
        }

        public Task<BigInteger> TreasuryFeeQueryAsync(TreasuryFeeFunction treasuryFeeFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TreasuryFeeFunction, BigInteger>(treasuryFeeFunction, blockParameter);
        }


        public virtual Task<BigInteger> TreasuryFeeQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TreasuryFeeFunction, BigInteger>(null, blockParameter);
        }

        public Task<string> AdminAddressQueryAsync(AdminAddressFunction adminAddressFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<AdminAddressFunction, string>(adminAddressFunction, blockParameter);
        }


        public virtual Task<string> AdminAddressQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<AdminAddressFunction, string>(null, blockParameter);
        }

        public virtual Task<string> BetBearRequestAsync(BetBearFunction betBearFunction)
        {
            return ContractHandler.SendRequestAsync(betBearFunction);
        }

        public virtual Task<TransactionReceipt> BetBearRequestAndWaitForReceiptAsync(BetBearFunction betBearFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(betBearFunction, cancellationToken);
        }

        public virtual Task<string> BetBearRequestAsync(BigInteger epoch)
        {
            var betBearFunction = new BetBearFunction();
            betBearFunction.Epoch = epoch;

            return ContractHandler.SendRequestAsync(betBearFunction);
        }

        public virtual Task<TransactionReceipt> BetBearRequestAndWaitForReceiptAsync(BigInteger epoch, CancellationTokenSource cancellationToken = null)
        {
            var betBearFunction = new BetBearFunction();
            betBearFunction.Epoch = epoch;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(betBearFunction, cancellationToken);
        }

        public virtual Task<string> BetBullRequestAsync(BetBullFunction betBullFunction)
        {
            return ContractHandler.SendRequestAsync(betBullFunction);
        }

        public virtual Task<TransactionReceipt> BetBullRequestAndWaitForReceiptAsync(BetBullFunction betBullFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(betBullFunction, cancellationToken);
        }

        public virtual Task<string> BetBullRequestAsync(BigInteger epoch)
        {
            var betBullFunction = new BetBullFunction();
            betBullFunction.Epoch = epoch;

            return ContractHandler.SendRequestAsync(betBullFunction);
        }

        public virtual Task<TransactionReceipt> BetBullRequestAndWaitForReceiptAsync(BigInteger epoch, CancellationTokenSource cancellationToken = null)
        {
            var betBullFunction = new BetBullFunction();
            betBullFunction.Epoch = epoch;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(betBullFunction, cancellationToken);
        }

        public virtual Task<string> ClaimRequestAsync(ClaimFunction claimFunction)
        {
            return ContractHandler.SendRequestAsync(claimFunction);
        }

        public virtual Task<TransactionReceipt> ClaimRequestAndWaitForReceiptAsync(ClaimFunction claimFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(claimFunction, cancellationToken);
        }

        public virtual Task<string> ClaimRequestAsync(List<BigInteger> epochs)
        {
            var claimFunction = new ClaimFunction();
            claimFunction.Epochs = epochs;

            return ContractHandler.SendRequestAsync(claimFunction);
        }

        public virtual Task<TransactionReceipt> ClaimRequestAndWaitForReceiptAsync(List<BigInteger> epochs, CancellationTokenSource cancellationToken = null)
        {
            var claimFunction = new ClaimFunction();
            claimFunction.Epochs = epochs;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(claimFunction, cancellationToken);
        }

        public Task<BigInteger> CurrentEpochQueryAsync(CurrentEpochFunction currentEpochFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CurrentEpochFunction, BigInteger>(currentEpochFunction, blockParameter);
        }


        public virtual Task<BigInteger> CurrentEpochQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CurrentEpochFunction, BigInteger>(null, blockParameter);
        }

        public virtual Task<string> EndRoundRequestAsync(EndRoundFunction endRoundFunction)
        {
            return ContractHandler.SendRequestAsync(endRoundFunction);
        }

        public virtual Task<TransactionReceipt> EndRoundRequestAndWaitForReceiptAsync(EndRoundFunction endRoundFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(endRoundFunction, cancellationToken);
        }

        public virtual Task<string> EndRoundRequestAsync(BigInteger epoch, BigInteger price)
        {
            var endRoundFunction = new EndRoundFunction();
            endRoundFunction.Epoch = epoch;
            endRoundFunction.Price = price;

            return ContractHandler.SendRequestAsync(endRoundFunction);
        }

        public virtual Task<TransactionReceipt> EndRoundRequestAndWaitForReceiptAsync(BigInteger epoch, BigInteger price, CancellationTokenSource cancellationToken = null)
        {
            var endRoundFunction = new EndRoundFunction();
            endRoundFunction.Epoch = epoch;
            endRoundFunction.Price = price;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(endRoundFunction, cancellationToken);
        }

        public virtual Task<LedgerOutputDTO> LedgerQueryAsync(LedgerFunction ledgerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<LedgerFunction, LedgerOutputDTO>(ledgerFunction, blockParameter);
        }

        public virtual Task<LedgerOutputDTO> LedgerQueryAsync(BigInteger returnValue1, string returnValue2, BlockParameter blockParameter = null)
        {
            var ledgerFunction = new LedgerFunction();
            ledgerFunction.ReturnValue1 = returnValue1;
            ledgerFunction.ReturnValue2 = returnValue2;

            return ContractHandler.QueryDeserializingToObjectAsync<LedgerFunction, LedgerOutputDTO>(ledgerFunction, blockParameter);
        }

        public virtual Task<string> LockRoundRequestAsync(LockRoundFunction lockRoundFunction)
        {
            return ContractHandler.SendRequestAsync(lockRoundFunction);
        }

        public virtual Task<TransactionReceipt> LockRoundRequestAndWaitForReceiptAsync(LockRoundFunction lockRoundFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(lockRoundFunction, cancellationToken);
        }

        public virtual Task<string> LockRoundRequestAsync(BigInteger epoch, BigInteger price)
        {
            var lockRoundFunction = new LockRoundFunction();
            lockRoundFunction.Epoch = epoch;
            lockRoundFunction.Price = price;

            return ContractHandler.SendRequestAsync(lockRoundFunction);
        }

        public virtual Task<TransactionReceipt> LockRoundRequestAndWaitForReceiptAsync(BigInteger epoch, BigInteger price, CancellationTokenSource cancellationToken = null)
        {
            var lockRoundFunction = new LockRoundFunction();
            lockRoundFunction.Epoch = epoch;
            lockRoundFunction.Price = price;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(lockRoundFunction, cancellationToken);
        }

        public virtual Task<RoundsOutputDTO> RoundsQueryAsync(RoundsFunction roundsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<RoundsFunction, RoundsOutputDTO>(roundsFunction, blockParameter);
        }

        public virtual Task<RoundsOutputDTO> RoundsQueryAsync(BigInteger returnValue1, BlockParameter blockParameter = null)
        {
            var roundsFunction = new RoundsFunction();
            roundsFunction.ReturnValue1 = returnValue1;

            return ContractHandler.QueryDeserializingToObjectAsync<RoundsFunction, RoundsOutputDTO>(roundsFunction, blockParameter);
        }

        public virtual Task<string> StartRoundRequestAsync(StartRoundFunction startRoundFunction)
        {
            return ContractHandler.SendRequestAsync(startRoundFunction);
        }

        public virtual Task<string> StartRoundRequestAsync()
        {
            return ContractHandler.SendRequestAsync<StartRoundFunction>();
        }

        public virtual Task<TransactionReceipt> StartRoundRequestAndWaitForReceiptAsync(StartRoundFunction startRoundFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(startRoundFunction, cancellationToken);
        }

        public virtual Task<TransactionReceipt> StartRoundRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync<StartRoundFunction>(null, cancellationToken);
        }

        public Task<BigInteger> TreasuryAmountQueryAsync(TreasuryAmountFunction treasuryAmountFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TreasuryAmountFunction, BigInteger>(treasuryAmountFunction, blockParameter);
        }


        public virtual Task<BigInteger> TreasuryAmountQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TreasuryAmountFunction, BigInteger>(null, blockParameter);
        }

        public Task<BigInteger> UserRoundsQueryAsync(UserRoundsFunction userRoundsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<UserRoundsFunction, BigInteger>(userRoundsFunction, blockParameter);
        }


        public virtual Task<BigInteger> UserRoundsQueryAsync(string returnValue1, BigInteger returnValue2, BlockParameter blockParameter = null)
        {
            var userRoundsFunction = new UserRoundsFunction();
            userRoundsFunction.ReturnValue1 = returnValue1;
            userRoundsFunction.ReturnValue2 = returnValue2;

            return ContractHandler.QueryAsync<UserRoundsFunction, BigInteger>(userRoundsFunction, blockParameter);
        }

        public virtual List<Type> GetAllFunctionTypes()
        {
            return new List<Type>
            {
                typeof(IntervalSecondsFunction),
                typeof(TreasuryFeeFunction),
                typeof(AdminAddressFunction),
                typeof(BetBearFunction),
                typeof(BetBullFunction),
                typeof(ClaimFunction),
                typeof(CurrentEpochFunction),
                typeof(EndRoundFunction),
                typeof(LedgerFunction),
                typeof(LockRoundFunction),
                typeof(RoundsFunction),
                typeof(StartRoundFunction),
                typeof(TreasuryAmountFunction),
                typeof(UserRoundsFunction)
            };
        }

        public virtual List<Type> GetAllEventTypes()
        {
            return new List<Type>
            {
                typeof(BetBearEventDTO),
                typeof(BetBullEventDTO),
                typeof(ClaimEventDTO),
                typeof(EndRoundEventDTO),
                typeof(LockRoundEventDTO),
                typeof(StartRoundEventDTO)
            };
        }

        public virtual List<Type> GetAllErrorTypes()
        {
            return new List<Type>
            {

            };
        }
    }
}
