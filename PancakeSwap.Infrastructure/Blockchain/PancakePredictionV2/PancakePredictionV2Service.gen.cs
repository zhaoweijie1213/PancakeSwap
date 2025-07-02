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

        public Task<BigInteger> MaxTreasuryFeeQueryAsync(MaxTreasuryFeeFunction maxTreasuryFeeFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<MaxTreasuryFeeFunction, BigInteger>(maxTreasuryFeeFunction, blockParameter);
        }


        public virtual Task<BigInteger> MaxTreasuryFeeQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<MaxTreasuryFeeFunction, BigInteger>(null, blockParameter);
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

        public Task<BigInteger> BufferSecondsQueryAsync(BufferSecondsFunction bufferSecondsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BufferSecondsFunction, BigInteger>(bufferSecondsFunction, blockParameter);
        }


        public virtual Task<BigInteger> BufferSecondsQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BufferSecondsFunction, BigInteger>(null, blockParameter);
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

        public virtual Task<string> ClaimTreasuryRequestAsync(ClaimTreasuryFunction claimTreasuryFunction)
        {
            return ContractHandler.SendRequestAsync(claimTreasuryFunction);
        }

        public virtual Task<string> ClaimTreasuryRequestAsync()
        {
            return ContractHandler.SendRequestAsync<ClaimTreasuryFunction>();
        }

        public virtual Task<TransactionReceipt> ClaimTreasuryRequestAndWaitForReceiptAsync(ClaimTreasuryFunction claimTreasuryFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(claimTreasuryFunction, cancellationToken);
        }

        public virtual Task<TransactionReceipt> ClaimTreasuryRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync<ClaimTreasuryFunction>(null, cancellationToken);
        }

        public Task<bool> ClaimableQueryAsync(ClaimableFunction claimableFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ClaimableFunction, bool>(claimableFunction, blockParameter);
        }


        public virtual Task<bool> ClaimableQueryAsync(BigInteger epoch, string user, BlockParameter blockParameter = null)
        {
            var claimableFunction = new ClaimableFunction();
            claimableFunction.Epoch = epoch;
            claimableFunction.User = user;

            return ContractHandler.QueryAsync<ClaimableFunction, bool>(claimableFunction, blockParameter);
        }

        public Task<BigInteger> CurrentEpochQueryAsync(CurrentEpochFunction currentEpochFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CurrentEpochFunction, BigInteger>(currentEpochFunction, blockParameter);
        }


        public virtual Task<BigInteger> CurrentEpochQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CurrentEpochFunction, BigInteger>(null, blockParameter);
        }

        public virtual Task<string> ExecuteRoundRequestAsync(ExecuteRoundFunction executeRoundFunction)
        {
            return ContractHandler.SendRequestAsync(executeRoundFunction);
        }

        public virtual Task<string> ExecuteRoundRequestAsync()
        {
            return ContractHandler.SendRequestAsync<ExecuteRoundFunction>();
        }

        public virtual Task<TransactionReceipt> ExecuteRoundRequestAndWaitForReceiptAsync(ExecuteRoundFunction executeRoundFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(executeRoundFunction, cancellationToken);
        }

        public virtual Task<TransactionReceipt> ExecuteRoundRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync<ExecuteRoundFunction>(null, cancellationToken);
        }

        public Task<bool> GenesisLockOnceQueryAsync(GenesisLockOnceFunction genesisLockOnceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GenesisLockOnceFunction, bool>(genesisLockOnceFunction, blockParameter);
        }


        public virtual Task<bool> GenesisLockOnceQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GenesisLockOnceFunction, bool>(null, blockParameter);
        }

        public virtual Task<string> GenesisLockRoundRequestAsync(GenesisLockRoundFunction genesisLockRoundFunction)
        {
            return ContractHandler.SendRequestAsync(genesisLockRoundFunction);
        }

        public virtual Task<string> GenesisLockRoundRequestAsync()
        {
            return ContractHandler.SendRequestAsync<GenesisLockRoundFunction>();
        }

        public virtual Task<TransactionReceipt> GenesisLockRoundRequestAndWaitForReceiptAsync(GenesisLockRoundFunction genesisLockRoundFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(genesisLockRoundFunction, cancellationToken);
        }

        public virtual Task<TransactionReceipt> GenesisLockRoundRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync<GenesisLockRoundFunction>(null, cancellationToken);
        }

        public Task<bool> GenesisStartOnceQueryAsync(GenesisStartOnceFunction genesisStartOnceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GenesisStartOnceFunction, bool>(genesisStartOnceFunction, blockParameter);
        }


        public virtual Task<bool> GenesisStartOnceQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GenesisStartOnceFunction, bool>(null, blockParameter);
        }

        public virtual Task<string> GenesisStartRoundRequestAsync(GenesisStartRoundFunction genesisStartRoundFunction)
        {
            return ContractHandler.SendRequestAsync(genesisStartRoundFunction);
        }

        public virtual Task<string> GenesisStartRoundRequestAsync()
        {
            return ContractHandler.SendRequestAsync<GenesisStartRoundFunction>();
        }

        public virtual Task<TransactionReceipt> GenesisStartRoundRequestAndWaitForReceiptAsync(GenesisStartRoundFunction genesisStartRoundFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(genesisStartRoundFunction, cancellationToken);
        }

        public virtual Task<TransactionReceipt> GenesisStartRoundRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync<GenesisStartRoundFunction>(null, cancellationToken);
        }

        public virtual Task<GetUserRoundsOutputDTO> GetUserRoundsQueryAsync(GetUserRoundsFunction getUserRoundsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetUserRoundsFunction, GetUserRoundsOutputDTO>(getUserRoundsFunction, blockParameter);
        }

        public virtual Task<GetUserRoundsOutputDTO> GetUserRoundsQueryAsync(string user, BigInteger cursor, BigInteger size, BlockParameter blockParameter = null)
        {
            var getUserRoundsFunction = new GetUserRoundsFunction();
            getUserRoundsFunction.User = user;
            getUserRoundsFunction.Cursor = cursor;
            getUserRoundsFunction.Size = size;

            return ContractHandler.QueryDeserializingToObjectAsync<GetUserRoundsFunction, GetUserRoundsOutputDTO>(getUserRoundsFunction, blockParameter);
        }

        public Task<BigInteger> GetUserRoundsLengthQueryAsync(GetUserRoundsLengthFunction getUserRoundsLengthFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetUserRoundsLengthFunction, BigInteger>(getUserRoundsLengthFunction, blockParameter);
        }


        public virtual Task<BigInteger> GetUserRoundsLengthQueryAsync(string user, BlockParameter blockParameter = null)
        {
            var getUserRoundsLengthFunction = new GetUserRoundsLengthFunction();
            getUserRoundsLengthFunction.User = user;

            return ContractHandler.QueryAsync<GetUserRoundsLengthFunction, BigInteger>(getUserRoundsLengthFunction, blockParameter);
        }

        public Task<BigInteger> IntervalSecondsQueryAsync(IntervalSecondsFunction intervalSecondsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IntervalSecondsFunction, BigInteger>(intervalSecondsFunction, blockParameter);
        }


        public virtual Task<BigInteger> IntervalSecondsQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IntervalSecondsFunction, BigInteger>(null, blockParameter);
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

        public Task<BigInteger> MinBetAmountQueryAsync(MinBetAmountFunction minBetAmountFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<MinBetAmountFunction, BigInteger>(minBetAmountFunction, blockParameter);
        }


        public virtual Task<BigInteger> MinBetAmountQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<MinBetAmountFunction, BigInteger>(null, blockParameter);
        }

        public Task<string> OperatorAddressQueryAsync(OperatorAddressFunction operatorAddressFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OperatorAddressFunction, string>(operatorAddressFunction, blockParameter);
        }


        public virtual Task<string> OperatorAddressQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OperatorAddressFunction, string>(null, blockParameter);
        }

        public Task<string> OracleQueryAsync(OracleFunction oracleFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OracleFunction, string>(oracleFunction, blockParameter);
        }


        public virtual Task<string> OracleQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OracleFunction, string>(null, blockParameter);
        }

        public Task<BigInteger> OracleLatestRoundIdQueryAsync(OracleLatestRoundIdFunction oracleLatestRoundIdFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OracleLatestRoundIdFunction, BigInteger>(oracleLatestRoundIdFunction, blockParameter);
        }


        public virtual Task<BigInteger> OracleLatestRoundIdQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OracleLatestRoundIdFunction, BigInteger>(null, blockParameter);
        }

        public Task<BigInteger> OracleUpdateAllowanceQueryAsync(OracleUpdateAllowanceFunction oracleUpdateAllowanceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OracleUpdateAllowanceFunction, BigInteger>(oracleUpdateAllowanceFunction, blockParameter);
        }


        public virtual Task<BigInteger> OracleUpdateAllowanceQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OracleUpdateAllowanceFunction, BigInteger>(null, blockParameter);
        }

        public Task<string> OwnerQueryAsync(OwnerFunction ownerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(ownerFunction, blockParameter);
        }


        public virtual Task<string> OwnerQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(null, blockParameter);
        }

        public virtual Task<string> PauseRequestAsync(PauseFunction pauseFunction)
        {
            return ContractHandler.SendRequestAsync(pauseFunction);
        }

        public virtual Task<string> PauseRequestAsync()
        {
            return ContractHandler.SendRequestAsync<PauseFunction>();
        }

        public virtual Task<TransactionReceipt> PauseRequestAndWaitForReceiptAsync(PauseFunction pauseFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(pauseFunction, cancellationToken);
        }

        public virtual Task<TransactionReceipt> PauseRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync<PauseFunction>(null, cancellationToken);
        }

        public Task<bool> PausedQueryAsync(PausedFunction pausedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<PausedFunction, bool>(pausedFunction, blockParameter);
        }


        public virtual Task<bool> PausedQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<PausedFunction, bool>(null, blockParameter);
        }

        public virtual Task<string> RecoverTokenRequestAsync(RecoverTokenFunction recoverTokenFunction)
        {
            return ContractHandler.SendRequestAsync(recoverTokenFunction);
        }

        public virtual Task<TransactionReceipt> RecoverTokenRequestAndWaitForReceiptAsync(RecoverTokenFunction recoverTokenFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(recoverTokenFunction, cancellationToken);
        }

        public virtual Task<string> RecoverTokenRequestAsync(string token, BigInteger amount)
        {
            var recoverTokenFunction = new RecoverTokenFunction();
            recoverTokenFunction.Token = token;
            recoverTokenFunction.Amount = amount;

            return ContractHandler.SendRequestAsync(recoverTokenFunction);
        }

        public virtual Task<TransactionReceipt> RecoverTokenRequestAndWaitForReceiptAsync(string token, BigInteger amount, CancellationTokenSource cancellationToken = null)
        {
            var recoverTokenFunction = new RecoverTokenFunction();
            recoverTokenFunction.Token = token;
            recoverTokenFunction.Amount = amount;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(recoverTokenFunction, cancellationToken);
        }

        public Task<bool> RefundableQueryAsync(RefundableFunction refundableFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<RefundableFunction, bool>(refundableFunction, blockParameter);
        }


        public virtual Task<bool> RefundableQueryAsync(BigInteger epoch, string user, BlockParameter blockParameter = null)
        {
            var refundableFunction = new RefundableFunction();
            refundableFunction.Epoch = epoch;
            refundableFunction.User = user;

            return ContractHandler.QueryAsync<RefundableFunction, bool>(refundableFunction, blockParameter);
        }

        public virtual Task<string> RenounceOwnershipRequestAsync(RenounceOwnershipFunction renounceOwnershipFunction)
        {
            return ContractHandler.SendRequestAsync(renounceOwnershipFunction);
        }

        public virtual Task<string> RenounceOwnershipRequestAsync()
        {
            return ContractHandler.SendRequestAsync<RenounceOwnershipFunction>();
        }

        public virtual Task<TransactionReceipt> RenounceOwnershipRequestAndWaitForReceiptAsync(RenounceOwnershipFunction renounceOwnershipFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(renounceOwnershipFunction, cancellationToken);
        }

        public virtual Task<TransactionReceipt> RenounceOwnershipRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync<RenounceOwnershipFunction>(null, cancellationToken);
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

        public virtual Task<string> SetAdminRequestAsync(SetAdminFunction setAdminFunction)
        {
            return ContractHandler.SendRequestAsync(setAdminFunction);
        }

        public virtual Task<TransactionReceipt> SetAdminRequestAndWaitForReceiptAsync(SetAdminFunction setAdminFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(setAdminFunction, cancellationToken);
        }

        public virtual Task<string> SetAdminRequestAsync(string adminAddress)
        {
            var setAdminFunction = new SetAdminFunction();
            setAdminFunction.AdminAddress = adminAddress;

            return ContractHandler.SendRequestAsync(setAdminFunction);
        }

        public virtual Task<TransactionReceipt> SetAdminRequestAndWaitForReceiptAsync(string adminAddress, CancellationTokenSource cancellationToken = null)
        {
            var setAdminFunction = new SetAdminFunction();
            setAdminFunction.AdminAddress = adminAddress;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(setAdminFunction, cancellationToken);
        }

        public virtual Task<string> SetBufferAndIntervalSecondsRequestAsync(SetBufferAndIntervalSecondsFunction setBufferAndIntervalSecondsFunction)
        {
            return ContractHandler.SendRequestAsync(setBufferAndIntervalSecondsFunction);
        }

        public virtual Task<TransactionReceipt> SetBufferAndIntervalSecondsRequestAndWaitForReceiptAsync(SetBufferAndIntervalSecondsFunction setBufferAndIntervalSecondsFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(setBufferAndIntervalSecondsFunction, cancellationToken);
        }

        public virtual Task<string> SetBufferAndIntervalSecondsRequestAsync(BigInteger bufferSeconds, BigInteger intervalSeconds)
        {
            var setBufferAndIntervalSecondsFunction = new SetBufferAndIntervalSecondsFunction();
            setBufferAndIntervalSecondsFunction.BufferSeconds = bufferSeconds;
            setBufferAndIntervalSecondsFunction.IntervalSeconds = intervalSeconds;

            return ContractHandler.SendRequestAsync(setBufferAndIntervalSecondsFunction);
        }

        public virtual Task<TransactionReceipt> SetBufferAndIntervalSecondsRequestAndWaitForReceiptAsync(BigInteger bufferSeconds, BigInteger intervalSeconds, CancellationTokenSource cancellationToken = null)
        {
            var setBufferAndIntervalSecondsFunction = new SetBufferAndIntervalSecondsFunction();
            setBufferAndIntervalSecondsFunction.BufferSeconds = bufferSeconds;
            setBufferAndIntervalSecondsFunction.IntervalSeconds = intervalSeconds;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(setBufferAndIntervalSecondsFunction, cancellationToken);
        }

        public virtual Task<string> SetMinBetAmountRequestAsync(SetMinBetAmountFunction setMinBetAmountFunction)
        {
            return ContractHandler.SendRequestAsync(setMinBetAmountFunction);
        }

        public virtual Task<TransactionReceipt> SetMinBetAmountRequestAndWaitForReceiptAsync(SetMinBetAmountFunction setMinBetAmountFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(setMinBetAmountFunction, cancellationToken);
        }

        public virtual Task<string> SetMinBetAmountRequestAsync(BigInteger minBetAmount)
        {
            var setMinBetAmountFunction = new SetMinBetAmountFunction();
            setMinBetAmountFunction.MinBetAmount = minBetAmount;

            return ContractHandler.SendRequestAsync(setMinBetAmountFunction);
        }

        public virtual Task<TransactionReceipt> SetMinBetAmountRequestAndWaitForReceiptAsync(BigInteger minBetAmount, CancellationTokenSource cancellationToken = null)
        {
            var setMinBetAmountFunction = new SetMinBetAmountFunction();
            setMinBetAmountFunction.MinBetAmount = minBetAmount;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(setMinBetAmountFunction, cancellationToken);
        }

        public virtual Task<string> SetOperatorRequestAsync(SetOperatorFunction setOperatorFunction)
        {
            return ContractHandler.SendRequestAsync(setOperatorFunction);
        }

        public virtual Task<TransactionReceipt> SetOperatorRequestAndWaitForReceiptAsync(SetOperatorFunction setOperatorFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(setOperatorFunction, cancellationToken);
        }

        public virtual Task<string> SetOperatorRequestAsync(string operatorAddress)
        {
            var setOperatorFunction = new SetOperatorFunction();
            setOperatorFunction.OperatorAddress = operatorAddress;

            return ContractHandler.SendRequestAsync(setOperatorFunction);
        }

        public virtual Task<TransactionReceipt> SetOperatorRequestAndWaitForReceiptAsync(string operatorAddress, CancellationTokenSource cancellationToken = null)
        {
            var setOperatorFunction = new SetOperatorFunction();
            setOperatorFunction.OperatorAddress = operatorAddress;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(setOperatorFunction, cancellationToken);
        }

        public virtual Task<string> SetOracleRequestAsync(SetOracleFunction setOracleFunction)
        {
            return ContractHandler.SendRequestAsync(setOracleFunction);
        }

        public virtual Task<TransactionReceipt> SetOracleRequestAndWaitForReceiptAsync(SetOracleFunction setOracleFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(setOracleFunction, cancellationToken);
        }

        public virtual Task<string> SetOracleRequestAsync(string oracle)
        {
            var setOracleFunction = new SetOracleFunction();
            setOracleFunction.Oracle = oracle;

            return ContractHandler.SendRequestAsync(setOracleFunction);
        }

        public virtual Task<TransactionReceipt> SetOracleRequestAndWaitForReceiptAsync(string oracle, CancellationTokenSource cancellationToken = null)
        {
            var setOracleFunction = new SetOracleFunction();
            setOracleFunction.Oracle = oracle;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(setOracleFunction, cancellationToken);
        }

        public virtual Task<string> SetOracleUpdateAllowanceRequestAsync(SetOracleUpdateAllowanceFunction setOracleUpdateAllowanceFunction)
        {
            return ContractHandler.SendRequestAsync(setOracleUpdateAllowanceFunction);
        }

        public virtual Task<TransactionReceipt> SetOracleUpdateAllowanceRequestAndWaitForReceiptAsync(SetOracleUpdateAllowanceFunction setOracleUpdateAllowanceFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(setOracleUpdateAllowanceFunction, cancellationToken);
        }

        public virtual Task<string> SetOracleUpdateAllowanceRequestAsync(BigInteger oracleUpdateAllowance)
        {
            var setOracleUpdateAllowanceFunction = new SetOracleUpdateAllowanceFunction();
            setOracleUpdateAllowanceFunction.OracleUpdateAllowance = oracleUpdateAllowance;

            return ContractHandler.SendRequestAsync(setOracleUpdateAllowanceFunction);
        }

        public virtual Task<TransactionReceipt> SetOracleUpdateAllowanceRequestAndWaitForReceiptAsync(BigInteger oracleUpdateAllowance, CancellationTokenSource cancellationToken = null)
        {
            var setOracleUpdateAllowanceFunction = new SetOracleUpdateAllowanceFunction();
            setOracleUpdateAllowanceFunction.OracleUpdateAllowance = oracleUpdateAllowance;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(setOracleUpdateAllowanceFunction, cancellationToken);
        }

        public virtual Task<string> SetTreasuryFeeRequestAsync(SetTreasuryFeeFunction setTreasuryFeeFunction)
        {
            return ContractHandler.SendRequestAsync(setTreasuryFeeFunction);
        }

        public virtual Task<TransactionReceipt> SetTreasuryFeeRequestAndWaitForReceiptAsync(SetTreasuryFeeFunction setTreasuryFeeFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(setTreasuryFeeFunction, cancellationToken);
        }

        public virtual Task<string> SetTreasuryFeeRequestAsync(BigInteger treasuryFee)
        {
            var setTreasuryFeeFunction = new SetTreasuryFeeFunction();
            setTreasuryFeeFunction.TreasuryFee = treasuryFee;

            return ContractHandler.SendRequestAsync(setTreasuryFeeFunction);
        }

        public virtual Task<TransactionReceipt> SetTreasuryFeeRequestAndWaitForReceiptAsync(BigInteger treasuryFee, CancellationTokenSource cancellationToken = null)
        {
            var setTreasuryFeeFunction = new SetTreasuryFeeFunction();
            setTreasuryFeeFunction.TreasuryFee = treasuryFee;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(setTreasuryFeeFunction, cancellationToken);
        }

        public virtual Task<string> TransferOwnershipRequestAsync(TransferOwnershipFunction transferOwnershipFunction)
        {
            return ContractHandler.SendRequestAsync(transferOwnershipFunction);
        }

        public virtual Task<TransactionReceipt> TransferOwnershipRequestAndWaitForReceiptAsync(TransferOwnershipFunction transferOwnershipFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(transferOwnershipFunction, cancellationToken);
        }

        public virtual Task<string> TransferOwnershipRequestAsync(string newOwner)
        {
            var transferOwnershipFunction = new TransferOwnershipFunction();
            transferOwnershipFunction.NewOwner = newOwner;

            return ContractHandler.SendRequestAsync(transferOwnershipFunction);
        }

        public virtual Task<TransactionReceipt> TransferOwnershipRequestAndWaitForReceiptAsync(string newOwner, CancellationTokenSource cancellationToken = null)
        {
            var transferOwnershipFunction = new TransferOwnershipFunction();
            transferOwnershipFunction.NewOwner = newOwner;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(transferOwnershipFunction, cancellationToken);
        }

        public Task<BigInteger> TreasuryAmountQueryAsync(TreasuryAmountFunction treasuryAmountFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TreasuryAmountFunction, BigInteger>(treasuryAmountFunction, blockParameter);
        }


        public virtual Task<BigInteger> TreasuryAmountQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TreasuryAmountFunction, BigInteger>(null, blockParameter);
        }

        public Task<BigInteger> TreasuryFeeQueryAsync(TreasuryFeeFunction treasuryFeeFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TreasuryFeeFunction, BigInteger>(treasuryFeeFunction, blockParameter);
        }


        public virtual Task<BigInteger> TreasuryFeeQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TreasuryFeeFunction, BigInteger>(null, blockParameter);
        }

        public virtual Task<string> UnpauseRequestAsync(UnpauseFunction unpauseFunction)
        {
            return ContractHandler.SendRequestAsync(unpauseFunction);
        }

        public virtual Task<string> UnpauseRequestAsync()
        {
            return ContractHandler.SendRequestAsync<UnpauseFunction>();
        }

        public virtual Task<TransactionReceipt> UnpauseRequestAndWaitForReceiptAsync(UnpauseFunction unpauseFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(unpauseFunction, cancellationToken);
        }

        public virtual Task<TransactionReceipt> UnpauseRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync<UnpauseFunction>(null, cancellationToken);
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

        public override List<Type> GetAllFunctionTypes()
        {
            return new List<Type>
            {
                typeof(MaxTreasuryFeeFunction),
                typeof(AdminAddressFunction),
                typeof(BetBearFunction),
                typeof(BetBullFunction),
                typeof(BufferSecondsFunction),
                typeof(ClaimFunction),
                typeof(ClaimTreasuryFunction),
                typeof(ClaimableFunction),
                typeof(CurrentEpochFunction),
                typeof(ExecuteRoundFunction),
                typeof(GenesisLockOnceFunction),
                typeof(GenesisLockRoundFunction),
                typeof(GenesisStartOnceFunction),
                typeof(GenesisStartRoundFunction),
                typeof(GetUserRoundsFunction),
                typeof(GetUserRoundsLengthFunction),
                typeof(IntervalSecondsFunction),
                typeof(LedgerFunction),
                typeof(MinBetAmountFunction),
                typeof(OperatorAddressFunction),
                typeof(OracleFunction),
                typeof(OracleLatestRoundIdFunction),
                typeof(OracleUpdateAllowanceFunction),
                typeof(OwnerFunction),
                typeof(PauseFunction),
                typeof(PausedFunction),
                typeof(RecoverTokenFunction),
                typeof(RefundableFunction),
                typeof(RenounceOwnershipFunction),
                typeof(RoundsFunction),
                typeof(SetAdminFunction),
                typeof(SetBufferAndIntervalSecondsFunction),
                typeof(SetMinBetAmountFunction),
                typeof(SetOperatorFunction),
                typeof(SetOracleFunction),
                typeof(SetOracleUpdateAllowanceFunction),
                typeof(SetTreasuryFeeFunction),
                typeof(TransferOwnershipFunction),
                typeof(TreasuryAmountFunction),
                typeof(TreasuryFeeFunction),
                typeof(UnpauseFunction),
                typeof(UserRoundsFunction)
            };
        }

        public override List<Type> GetAllEventTypes()
        {
            return new List<Type>
            {
                typeof(BetBearEventDTO),
                typeof(BetBullEventDTO),
                typeof(ClaimEventDTO),
                typeof(EndRoundEventDTO),
                typeof(LockRoundEventDTO),
                typeof(NewAdminAddressEventDTO),
                typeof(NewBufferAndIntervalSecondsEventDTO),
                typeof(NewMinBetAmountEventDTO),
                typeof(NewOperatorAddressEventDTO),
                typeof(NewOracleEventDTO),
                typeof(NewOracleUpdateAllowanceEventDTO),
                typeof(NewTreasuryFeeEventDTO),
                typeof(OwnershipTransferredEventDTO),
                typeof(PauseEventDTO),
                typeof(PausedEventDTO),
                typeof(RewardsCalculatedEventDTO),
                typeof(StartRoundEventDTO),
                typeof(TokenRecoveryEventDTO),
                typeof(TreasuryClaimEventDTO),
                typeof(UnpauseEventDTO),
                typeof(UnpausedEventDTO)
            };
        }

        public override List<Type> GetAllErrorTypes()
        {
            return new List<Type>
            {

            };
        }
    }
}
