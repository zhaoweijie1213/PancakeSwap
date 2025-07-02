// scripts/keeper.ts ----------------------------------------------------------
import * as dotenv from 'dotenv';
dotenv.config();
import { ethers, network } from 'hardhat';
import type { Contract } from 'ethers'; // ← 仅引入类型

const RPC = process.env.BSC_RPC || 'http://127.0.0.1:8545';
const OPERATOR_PK = process.env.OPERATOR_PK || '';
const PRED_ADDR = process.env.CONTRACT_ADDR_LOCAL!;
const ORACLE_ADDR = process.env.MOCK_ORACLE_ADDR; // 本地链 mock 时用
const INTERVAL = Number(process.env.INTERVAL_SECONDS) || 300;
const BUFFER = Number(process.env.BUFFER_SECONDS) || 60;

if (!OPERATOR_PK) throw new Error('OPERATOR_PK 未配置');
if (!PRED_ADDR) throw new Error('CONTRACT_ADDR_LOCAL 未配置');

const provider = new ethers.JsonRpcProvider(RPC);
const wallet = new ethers.Wallet(OPERATOR_PK, provider);

async function delay(ms: number) {
    return new Promise((res) => setTimeout(res, ms));
}

async function fastForward(seconds: number) {
    // 仅在本地 Hardhat (31337) 时加速
    const net = await provider.getNetwork();
    if (net.chainId === 31337n) {
        await provider.send('evm_increaseTime', [seconds]);
        await provider.send('evm_mine', []);
    } else {
        await delay(seconds * 1000);
    }
}

async function randomPrice(base: number): Promise<bigint> {
    // ±1 % 浮动
    const delta = base * (Math.random() * 0.02 - 0.01);
    return BigInt(Math.floor((base + delta) * 1e8));
}

async function main() {
    console.log('⏲️  Keeper bot starting ...');

    // 连接 Prediction 合约
    const predAbi = (
        await ethers.getContractFactory('PancakePredictionV2')
    ).interface.formatJson() as string;
    const pred = new ethers.Contract(PRED_ADDR, predAbi, wallet);

    // 连接 Mock oracle（本地链才有）
    let mock: Contract | null = null;
    if (ORACLE_ADDR) {
        const mockAbi = ['function updateAnswer(int256) external'];
        mock = new ethers.Contract(ORACLE_ADDR, mockAbi, wallet);
    }

    // -------------------- 创世两步 --------------------
    const genesisStartOnce: boolean = await pred.genesisStartOnce();
    const genesisLockOnce: boolean = await pred.genesisLockOnce();

    if (!genesisStartOnce) {
        console.log('⚡  Calling genesisStartRound()');
        await (await pred.genesisStartRound()).wait();
    }

    await fastForward(INTERVAL); // 等待/快进 1 个 interval

    if (!genesisLockOnce) {
        // 第一次锁盘前给个初始价格
        if (mock) await mock.updateAnswer(await randomPrice(300));
        console.log('⚡  Calling genesisLockRound()');
        await (await pred.genesisLockRound()).wait();
    }

    await fastForward(INTERVAL); // 进入第 2 轮正常阶段

    console.log('✅  Genesis complete. Entering execute loop …');

    // -------------------- 循环执行 --------------------
    const LOOP = INTERVAL + BUFFER + 5; // 再加 5 秒冗余

    // eslint-disable-next-line no-constant-condition
    while (true) {
        try {
            // optional: 更新价格，确保 oracleUpdateAllowance 不超时
            if (mock) await mock.updateAnswer(await randomPrice(300));

            const tx = await pred.executeRound();
            console.log(new Date().toLocaleTimeString(), 'executeRound TX', tx.hash);
            await tx.wait();
            console.log('   ↳  round settled ✅');
        } catch (e: any) {
            console.error('❌ executeRound 失败:', e.reason || e.message);
        }

        await fastForward(LOOP);
    }
}

main().catch((err) => {
    console.error(err);
    process.exitCode = 1;
});
