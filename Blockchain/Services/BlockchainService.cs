﻿using Blockchain.Models;
<<<<<<< HEAD
using Blockchain.Utils;
using Newtonsoft.Json;
=======
using Microsoft.Extensions.Options;
>>>>>>> ccb7bc73c3bf0e281086d90435870d06dad976b4
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
        private AppSettings appSettings;

        public BlockchainService(IDBService dbService, IOptions<AppSettings> appSettings)
        {
            this.dbService = dbService;
            this.appSettings = appSettings.Value;
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

            dbService.SetLastBlock(new MinedBlockInfo {
                DateCreated = DateTime.Now,
                Difficulty = appSettings.Difficulty,
                Index = 0,
                MinedBy = "Mr. Bean",
                Nonce = 512,
                PreviousBlockHash = "",
                Transactions = new List<Transaction>()
            });
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
            //1. Create block mining candidate
            //2. Add record to the Node Mining Jobs (address => Block)

            var info = new MiningBlockInfo {
                Difficulty = appSettings.Difficulty,
                Index = dbService.GetLastBlock().Index + 1,
                MinedBy = address,
                PreviousBlockHash = dbService.GetLastBlock().BlockHash,
                Transactions = dbService.GetTransactions()
            };

            dbService.Set(info.Id, info);

            return info;
        }

        public SubmitBlockResponse SubmitBlockInfo(string blockId, MinedBlockInfo data)
        {
            //TODO: Implement
            var info = dbService.Get(blockId);
            if (null == info)
            {
                return new SubmitBlockResponse
                {
                    Status = "Error",
                    Message = "Wrong blockId!"
                };
            }

            if (data.BlockHash.StartsWith("".PadLeft(appSettings.Difficulty, '0')))
            {
                return new SubmitBlockResponse
                {
                    Status = "Success",
                    Message = $"Block is valid"
                };
            }
            else
            {
                return new SubmitBlockResponse
                {
                    Status = "Error",
                    Message = $"Hash must start with {appSettings.Difficulty} zeroes"
                };
            }
        }
    }
}
