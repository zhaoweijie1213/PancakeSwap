import { ethers } from 'hardhat';

async function main() {
    const Mock = await ethers.getContractFactory('MockAggregator');

    // 初始价格 300 USD，8 位小数
    const initPrice = ethers.parseUnits('300', 8); // 同样得到 30000000000

    const mock = await Mock.deploy(initPrice);
    await mock.waitForDeployment();
    console.log('✅ MockAggregator 部署成功：', mock.target);
}

main().catch((e) => {
    console.error(e);
    process.exitCode = 1;
});
