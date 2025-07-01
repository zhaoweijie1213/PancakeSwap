// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

/// @title Simplified Pancake Prediction V2
/// @notice Fork with 180-second rounds, 4% treasury fee and BNB-only betting.
contract PancakePredictionV2 {
    enum Position { Bull, Bear }

    struct Round {
        uint256 startTimestamp;
        uint256 lockTimestamp;
        uint256 closeTimestamp;
        int256 lockPrice;
        int256 closePrice;
        uint256 totalAmount;
        uint256 bullAmount;
        uint256 bearAmount;
        bool oracleCalled;
    }

    struct BetInfo {
        Position position;
        uint256 amount;
        bool claimed;
    }

    uint256 public constant INTERVAL_SECONDS = 180;
    uint256 public constant TREASURY_FEE = 4; // 4%

    uint256 public currentEpoch;
    address public adminAddress;
    uint256 public treasuryAmount;

    mapping(uint256 => Round) public rounds;
    mapping(uint256 => mapping(address => BetInfo)) public ledger;
    mapping(address => uint256[]) public userRounds;

    modifier onlyAdmin() {
        require(msg.sender == adminAddress, "Not admin");
        _;
    }

    modifier notContract() {
        require(msg.sender == tx.origin, "Contract not allowed");
        _;
    }

    event StartRound(uint256 indexed epoch);
    event LockRound(uint256 indexed epoch, int256 price);
    event EndRound(uint256 indexed epoch, int256 price);
    event BetBull(address indexed sender, uint256 indexed epoch, uint256 amount);
    event BetBear(address indexed sender, uint256 indexed epoch, uint256 amount);
    event Claim(address indexed sender, uint256 indexed epoch, uint256 amount);

    constructor() {
        adminAddress = msg.sender;
    }

    function startRound() external onlyAdmin {
        currentEpoch += 1;
        Round storage round = rounds[currentEpoch];
        round.startTimestamp = block.timestamp;
        round.lockTimestamp = block.timestamp + INTERVAL_SECONDS;
        round.closeTimestamp = block.timestamp + (INTERVAL_SECONDS * 2);
        emit StartRound(currentEpoch);
    }

    function lockRound(uint256 epoch, int256 price) external onlyAdmin {
        Round storage round = rounds[epoch];
        require(block.timestamp >= round.lockTimestamp, "Too early");
        round.lockPrice = price;
        emit LockRound(epoch, price);
    }

    function endRound(uint256 epoch, int256 price) external onlyAdmin {
        Round storage round = rounds[epoch];
        require(block.timestamp >= round.closeTimestamp, "Too early");
        round.closePrice = price;
        round.oracleCalled = true;
        emit EndRound(epoch, price);
    }

    function betBull(uint256 epoch) external payable notContract {
        _bet(epoch, Position.Bull);
    }

    function betBear(uint256 epoch) external payable notContract {
        _bet(epoch, Position.Bear);
    }

    function claim(uint256[] calldata epochs) external notContract {
        uint256 reward;
        for (uint256 i = 0; i < epochs.length; i++) {
            uint256 epoch = epochs[i];
            BetInfo storage betInfo = ledger[epoch][msg.sender];
            Round storage round = rounds[epoch];

            require(round.oracleCalled, "Round not ended");
            require(!betInfo.claimed, "Already claimed");
            require(betInfo.amount > 0, "No bet");

            bool won = (betInfo.position == Position.Bull && round.closePrice > round.lockPrice) ||
                (betInfo.position == Position.Bear && round.closePrice < round.lockPrice);

            if (won) {
                uint256 total = betInfo.position == Position.Bull ? round.bullAmount : round.bearAmount;
                uint256 rewardAmount = (betInfo.amount * round.totalAmount * (100 - TREASURY_FEE)) / total / 100;
                reward += rewardAmount;
            }
            betInfo.claimed = true;
        }
        if (reward > 0) {
            payable(msg.sender).transfer(reward);
            emit Claim(msg.sender, 0, reward);
        }
    }

    function _bet(uint256 epoch, Position position) internal {
        Round storage round = rounds[epoch];
        require(block.timestamp >= round.startTimestamp && block.timestamp < round.lockTimestamp, "Bet time expired");
        require(ledger[epoch][msg.sender].amount == 0, "Already bet");
        uint256 amount = msg.value;
        require(amount > 0, "Bet amount > 0");

        BetInfo storage betInfo = ledger[epoch][msg.sender];
        betInfo.position = position;
        betInfo.amount = amount;
        userRounds[msg.sender].push(epoch);

        round.totalAmount += amount;
        if (position == Position.Bull) {
            round.bullAmount += amount;
            emit BetBull(msg.sender, epoch, amount);
        } else {
            round.bearAmount += amount;
            emit BetBear(msg.sender, epoch, amount);
        }
    }

    receive() external payable {}
}

