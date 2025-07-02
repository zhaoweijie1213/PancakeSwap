// scripts/keeper.ts
import * as dotenv from 'dotenv';
dotenv.config();
import { ethers } from 'hardhat';

async function main() {
    const rpc = process.env.BSC_RPC || 'http://127.0.0.1:8545';
    const pk = process.env.OPERATOR_PK || ''; // operator 私钥
    const addr = process.env.CONTRACT_ADDR_LOCAL!; // Prediction 地址

    if (!pk) throw new Error('OPERATOR_PK 未配置');

    // 1 连接链
    const provider = new ethers.JsonRpcProvider(rpc);
    const wallet = new ethers.Wallet(pk, provider);

    // 2 连接合约
    const pred = await ethers.getContractAt('PancakePredictionV2', addr, wallet);

    // 3 定时任务：每 5 min+缓冲 多跑一次
    setInterval(async () => {
        try {
            const tx = await pred.executeRound();
            console.log(`[${new Date().toLocaleTimeString()}] executeRound TX: ${tx.hash}`);
            await tx.wait();
        } catch (e: any) {
            console.error('executeRound 失败:', e.reason || e.message);
        }
    }, (300 + 10) * 1000); // intervalSeconds 300 + 10 秒冗余
}

main().catch(console.error);
