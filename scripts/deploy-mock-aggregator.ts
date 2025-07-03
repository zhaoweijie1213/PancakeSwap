import { ethers } from 'hardhat';

async function main() {
    const decimals = 8;
    const initialPrice = ethers.parseUnits('300', decimals);

    const Aggregator = await ethers.getContractFactory('MockV3Aggregator');
    const aggregator = await Aggregator.deploy(decimals, initialPrice);

    await aggregator.waitForDeployment();
    console.log('✅ MockV3Aggregator 部署成功：', aggregator.target);
}

main().catch((err) => {
    console.error(err);
    process.exitCode = 1;
});
