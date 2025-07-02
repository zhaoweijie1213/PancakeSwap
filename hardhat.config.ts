import { HardhatUserConfig } from 'hardhat/config';
import '@nomicfoundation/hardhat-toolbox';
import * as dotenv from 'dotenv';

dotenv.config();

const config: HardhatUserConfig = {
    solidity: {
        version: '0.8.20',
        settings: {
            optimizer: { enabled: true, runs: 200 },
        },
    },
    networks: {
        localhost: {
            url: process.env.LOCAL_RPC || 'http://127.0.0.1:8545',
            chainId: 31337,
            accounts: process.env.LOCAL_MNEMONIC
                ? { mnemonic: process.env.LOCAL_MNEMONIC }
                : process.env.LOCAL_PK
                ? [process.env.LOCAL_PK]
                : [],
        },
        bsctest: {
            url: process.env.BSC_TESTNET_RPC || '',
            chainId: 97,
            accounts: process.env.PRIVATE_KEY ? [process.env.PRIVATE_KEY] : [],
        },
    },
};

export default config;
