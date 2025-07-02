import { ethers } from 'hardhat';

async function main() {
    const Prediction = await ethers.getContractFactory('PancakePredictionV2');
    const oracle = '0x0567F232…'; // BNB/USD 预言机
    const admin = '0xYourAdmin';
    const operator = '0xYourKeeperBot';
    const contract = await Prediction.deploy(
        oracle,
        admin,
        operator,
        300, // _intervalSeconds  ← 改成 300 秒
        60, // _bufferSeconds    ← 提前/延迟容忍 60 s
        ethers.parseEther('0.01'), // _minBetAmount     ← 0.01 BNB
        300, // _oracleUpdateAllowance
        400 // _treasuryFee = 4 %
    );
    await contract.waitForDeployment();
    console.log('Prediction deployed:', contract.target);
}

main().catch((error) => {
    console.error(error);
    process.exitCode = 1;
});
