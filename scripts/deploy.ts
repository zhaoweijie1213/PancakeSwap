// deploy.ts
import * as dotenv from 'dotenv';
dotenv.config();
import { ethers, network } from 'hardhat';

async function main() {
    // 根据当前 network 选合适的 oracle
    // Chainlink BNB / USD 预言机合约地址
    const oracle = network.name === 'bsctest' ? process.env.ORACLE_TEST : process.env.ORACLE_MAIN;
    //项目“超级管理员”帐号
    const admin = process.env.ADMIN!;
    //负责 定时执行 executeRound() 的机器人地址
    const operator = process.env.OPERATOR!;

    const Prediction = await ethers.getContractFactory('PancakePredictionV2');
    const contract = await Prediction.deploy(
        oracle,
        admin,
        operator,
        300, // intervalSeconds
        60, // bufferSeconds
        ethers.parseEther('0.01'), // minBetAmount
        300, // oracleUpdateAllowance
        400 // treasuryFee (4 %)
    );

    await contract.waitForDeployment();
    console.log('Prediction deployed at:', contract.target);
}

main().catch((e) => {
    console.error(e);
    process.exitCode = 1;
});
