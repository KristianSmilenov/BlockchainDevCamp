using Blockchain.Models;
using Blockchain.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace Blockchain.Services
{
    public class BlockchainService : IBlockchainService
    {
        private BlockchainInfo info;
        private List<Block> blocks;
        private List<string> peers;
        private List<Transaction> pendingTransactions;
        private IDBService dbService;

        public BlockchainService(IDBService dbService)
        {
            this.dbService = dbService;
            info = new BlockchainInfo("Overwatch Blockchain", "Genesis");
            blocks = new List<Block>();
            peers = new List<string>();
            pendingTransactions = new List<Transaction>();
            
            GenerateGenesisBlock();
        }

        private void GenerateGenesisBlock()
        {
            //TODO: Implement
            //Note: Give faucet some coins in the Genesis block
            //throw new NotImplementedException();
        }

        public BlockchainInfo GetBlockchainInfo()
        {
            return info;
        }
        
        public List<Block> GetBlocks()
        {
            return blocks;
        }
        
        public Block GetBlock(int index)
        {
            if(blocks.Count + 1 < index)
                return null;
            return blocks.ElementAt(index);
        }
        
        public void NotifyBlock(int index)
        {
            throw new NotImplementedException();
        }

        public TransactionHashInfo CreateTransaction(TransactionDataSigned signedData)
        {
            //TODO: Implement
            ValidateTransaction(signedData);
            return new TransactionHashInfo();
        }

        private bool ValidateTransaction(TransactionDataSigned signedData)
        {
            string privateKey = "5KQwrPbwdL6PhXujxW37FSSQZ1JiwsST4cqQzDeyXtP78zkvFD3";
            //TODO: Test CryptoUtils BouncyCastle.NetCore
            //TODO: Cryptography.ECDSA.Secp256k1
            return false;
        }
        
        public Balance GetBalance(string address)
        {
            //TODO: Implement
            return new Balance();
        }

        public Transaction GetTransaction(string transactionHash)
        {
            //TODO: Implement
            return new Transaction();
        }

        public List<string> GetPeers()
        {
            return peers;
        }

        public void AddPeer(string peerUrl)
        {
            if(!peers.Contains(peerUrl))
                peers.Add(peerUrl);
        }

        public MiningBlockInfo GetMiningBlockInfo(string address)
        {
            //TODO: Implement

            //1. Create block mining candidate
            //2. Add record to the Node Mining Jobs (address => Block)

            return new MiningBlockInfo();

        }

        public SubmitBlockResponse SubmitBlockInfo(string address, MinedBlockInfo data)
        {
            //TODO: Implement
            return new SubmitBlockResponse();
        }
    }
}
