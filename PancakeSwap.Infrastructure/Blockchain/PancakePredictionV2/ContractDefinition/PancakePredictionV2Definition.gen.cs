using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts;
using System.Threading;

namespace PancakeSwap.Infrastructure.Blockchain.PancakePredictionV2.ContractDefinition
{


    public partial class PancakePredictionV2Deployment : PancakePredictionV2DeploymentBase
    {
        public PancakePredictionV2Deployment() : base(BYTECODE) { }
        public PancakePredictionV2Deployment(string byteCode) : base(byteCode) { }
    }

    public class PancakePredictionV2DeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "";
        public PancakePredictionV2DeploymentBase() : base(BYTECODE) { }
        public PancakePredictionV2DeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class IntervalSecondsFunction : IntervalSecondsFunctionBase { }

    [Function("INTERVAL_SECONDS", "uint256")]
    public class IntervalSecondsFunctionBase : FunctionMessage
    {

    }

    public partial class TreasuryFeeFunction : TreasuryFeeFunctionBase { }

    [Function("TREASURY_FEE", "uint256")]
    public class TreasuryFeeFunctionBase : FunctionMessage
    {

    }

    public partial class AdminAddressFunction : AdminAddressFunctionBase { }

    [Function("adminAddress", "address")]
    public class AdminAddressFunctionBase : FunctionMessage
    {

    }

    public partial class BetBearFunction : BetBearFunctionBase { }

    [Function("betBear")]
    public class BetBearFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "epoch", 1)]
        public virtual BigInteger Epoch { get; set; }
    }

    public partial class BetBullFunction : BetBullFunctionBase { }

    [Function("betBull")]
    public class BetBullFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "epoch", 1)]
        public virtual BigInteger Epoch { get; set; }
    }

    public partial class ClaimFunction : ClaimFunctionBase { }

    [Function("claim")]
    public class ClaimFunctionBase : FunctionMessage
    {
        [Parameter("uint256[]", "epochs", 1)]
        public virtual List<BigInteger> Epochs { get; set; }
    }

    public partial class CurrentEpochFunction : CurrentEpochFunctionBase { }

    [Function("currentEpoch", "uint256")]
    public class CurrentEpochFunctionBase : FunctionMessage
    {

    }

    public partial class EndRoundFunction : EndRoundFunctionBase { }

    [Function("endRound")]
    public class EndRoundFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "epoch", 1)]
        public virtual BigInteger Epoch { get; set; }
        [Parameter("int256", "price", 2)]
        public virtual BigInteger Price { get; set; }
    }

    public partial class LedgerFunction : LedgerFunctionBase { }

    [Function("ledger", typeof(LedgerOutputDTO))]
    public class LedgerFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
        [Parameter("address", "", 2)]
        public virtual string ReturnValue2 { get; set; }
    }

    public partial class LockRoundFunction : LockRoundFunctionBase { }

    [Function("lockRound")]
    public class LockRoundFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "epoch", 1)]
        public virtual BigInteger Epoch { get; set; }
        [Parameter("int256", "price", 2)]
        public virtual BigInteger Price { get; set; }
    }

    public partial class RoundsFunction : RoundsFunctionBase { }

    [Function("rounds", typeof(RoundsOutputDTO))]
    public class RoundsFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class StartRoundFunction : StartRoundFunctionBase { }

    [Function("startRound")]
    public class StartRoundFunctionBase : FunctionMessage
    {

    }

    public partial class TreasuryAmountFunction : TreasuryAmountFunctionBase { }

    [Function("treasuryAmount", "uint256")]
    public class TreasuryAmountFunctionBase : FunctionMessage
    {

    }

    public partial class UserRoundsFunction : UserRoundsFunctionBase { }

    [Function("userRounds", "uint256")]
    public class UserRoundsFunctionBase : FunctionMessage
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
        [Parameter("uint256", "", 2)]
        public virtual BigInteger ReturnValue2 { get; set; }
    }

    public partial class BetBearEventDTO : BetBearEventDTOBase { }

    [Event("BetBear")]
    public class BetBearEventDTOBase : IEventDTO
    {
        [Parameter("address", "sender", 1, true)]
        public virtual string Sender { get; set; }
        [Parameter("uint256", "epoch", 2, true)]
        public virtual BigInteger Epoch { get; set; }
        [Parameter("uint256", "amount", 3, false)]
        public virtual BigInteger Amount { get; set; }
    }

    public partial class BetBullEventDTO : BetBullEventDTOBase { }

    [Event("BetBull")]
    public class BetBullEventDTOBase : IEventDTO
    {
        [Parameter("address", "sender", 1, true)]
        public virtual string Sender { get; set; }
        [Parameter("uint256", "epoch", 2, true)]
        public virtual BigInteger Epoch { get; set; }
        [Parameter("uint256", "amount", 3, false)]
        public virtual BigInteger Amount { get; set; }
    }

    public partial class ClaimEventDTO : ClaimEventDTOBase { }

    [Event("Claim")]
    public class ClaimEventDTOBase : IEventDTO
    {
        [Parameter("address", "sender", 1, true)]
        public virtual string Sender { get; set; }
        [Parameter("uint256", "epoch", 2, true)]
        public virtual BigInteger Epoch { get; set; }
        [Parameter("uint256", "amount", 3, false)]
        public virtual BigInteger Amount { get; set; }
    }

    public partial class EndRoundEventDTO : EndRoundEventDTOBase { }

    [Event("EndRound")]
    public class EndRoundEventDTOBase : IEventDTO
    {
        [Parameter("uint256", "epoch", 1, true)]
        public virtual BigInteger Epoch { get; set; }
        [Parameter("int256", "price", 2, false)]
        public virtual BigInteger Price { get; set; }
    }

    public partial class LockRoundEventDTO : LockRoundEventDTOBase { }

    [Event("LockRound")]
    public class LockRoundEventDTOBase : IEventDTO
    {
        [Parameter("uint256", "epoch", 1, true)]
        public virtual BigInteger Epoch { get; set; }
        [Parameter("int256", "price", 2, false)]
        public virtual BigInteger Price { get; set; }
    }

    public partial class StartRoundEventDTO : StartRoundEventDTOBase { }

    [Event("StartRound")]
    public class StartRoundEventDTOBase : IEventDTO
    {
        [Parameter("uint256", "epoch", 1, true)]
        public virtual BigInteger Epoch { get; set; }
    }

    public partial class IntervalSecondsOutputDTO : IntervalSecondsOutputDTOBase { }

    [FunctionOutput]
    public class IntervalSecondsOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class TreasuryFeeOutputDTO : TreasuryFeeOutputDTOBase { }

    [FunctionOutput]
    public class TreasuryFeeOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class AdminAddressOutputDTO : AdminAddressOutputDTOBase { }

    [FunctionOutput]
    public class AdminAddressOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }







    public partial class CurrentEpochOutputDTO : CurrentEpochOutputDTOBase { }

    [FunctionOutput]
    public class CurrentEpochOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }



    public partial class LedgerOutputDTO : LedgerOutputDTOBase { }

    [FunctionOutput]
    public class LedgerOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint8", "position", 1)]
        public virtual byte Position { get; set; }
        [Parameter("uint256", "amount", 2)]
        public virtual BigInteger Amount { get; set; }
        [Parameter("bool", "claimed", 3)]
        public virtual bool Claimed { get; set; }
    }



    public partial class RoundsOutputDTO : RoundsOutputDTOBase { }

    [FunctionOutput]
    public class RoundsOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "startTimestamp", 1)]
        public virtual BigInteger StartTimestamp { get; set; }
        [Parameter("uint256", "lockTimestamp", 2)]
        public virtual BigInteger LockTimestamp { get; set; }
        [Parameter("uint256", "closeTimestamp", 3)]
        public virtual BigInteger CloseTimestamp { get; set; }
        [Parameter("int256", "lockPrice", 4)]
        public virtual BigInteger LockPrice { get; set; }
        [Parameter("int256", "closePrice", 5)]
        public virtual BigInteger ClosePrice { get; set; }
        [Parameter("uint256", "totalAmount", 6)]
        public virtual BigInteger TotalAmount { get; set; }
        [Parameter("uint256", "bullAmount", 7)]
        public virtual BigInteger BullAmount { get; set; }
        [Parameter("uint256", "bearAmount", 8)]
        public virtual BigInteger BearAmount { get; set; }
        [Parameter("bool", "oracleCalled", 9)]
        public virtual bool OracleCalled { get; set; }
    }



    public partial class TreasuryAmountOutputDTO : TreasuryAmountOutputDTOBase { }

    [FunctionOutput]
    public class TreasuryAmountOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class UserRoundsOutputDTO : UserRoundsOutputDTOBase { }

    [FunctionOutput]
    public class UserRoundsOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }
}
