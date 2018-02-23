using Blockchain.Models;
using BlockchainCore.Utils;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blockchain.Services
{
    public class BlockchainService : IBlockchainService
    {
        private BlockchainInfo info;
        private List<string> peers;
        private List<Transaction> pendingTransactions;
        private IDBService dbService;
        private AppSettings appSettings;

        public BlockchainService(IDBService dbService, IOptions<AppSettings> appSettings)
        {
            this.dbService = dbService;
            this.appSettings = appSettings.Value;
            info = new BlockchainInfo("Overwatch Blockchain", "Genesis");
            peers = new List<string>();
            pendingTransactions = new List<Transaction>();
            
            GenerateGenesisBlock();
        }

        private void GenerateGenesisBlock()
        {
            //TODO: Give faucet some coins in the Genesis block

            dbService.AddBlock(
                new MinedBlockInfo
                {
                    Difficulty = appSettings.Difficulty,
                    Index = 0,
                    MinedBy = "Mr. Bean",
                    PreviousBlockHash = "",
                    Transactions = new List<Transaction>(),
                    DateCreated = DateTime.Now,
                    Nonce = 0
                });
        }

        public BlockchainInfo GetBlockchainInfo()
        {
            return info;
        }
        
        public List<MinedBlockInfoResponse> GetBlocks()
        {
            return dbService.GetAllBlocks().ConvertAll(b => MinedBlockInfoResponse.FromMinedBlockInfo(b));
        }
        
        public MinedBlockInfoResponse GetBlock(int index)
        {
            return MinedBlockInfoResponse.FromMinedBlockInfo(dbService.GetAllBlocks()[index]);
        }
        
        public void NotifyBlock(int index)
        {
            throw new NotImplementedException();
        }

        public TransactionHashInfo CreateTransaction(TransactionDataSigned signedData)
        {
            //TODO: Implement
            return new TransactionHashInfo() { IsValid = ValidateTransaction(signedData), DateReceived = DateTime.UtcNow, TransactionHash = "" }  ;
        }

        private bool ValidateTransaction(TransactionDataSigned signedData)
        {
            TransactionData toValidate = new TransactionData(signedData);
            string messageData = JsonConvert.SerializeObject(toValidate);
            
            // Validate received message data
            byte[] senderPublicKey = CryptoUtils.HexToByteArray(signedData.SenderPubKey);
            byte[] senderSignature = CryptoUtils.HexToByteArray(signedData.SenderSignature);
            byte[] messageDataHash = CryptoUtils.GetSha256Bytes(messageData);
            bool isTestValid = CryptoUtils.BouncyCastleVerify(messageDataHash, senderSignature, senderPublicKey);

            // Server side test
            string message = "Some super cool message";
            byte[] privateKey = CryptoUtils.CreateNewPrivateKey();
            byte[] publicKey = CryptoUtils.GetPublicFor(privateKey);
            byte[] msgHash = CryptoUtils.GetSha256Bytes(message);
            byte[] signedMessage = CryptoUtils.BouncyCastleSign(msgHash, privateKey);
            bool isValid = CryptoUtils.BouncyCastleVerify(msgHash, signedMessage, publicKey);
            string privateKeyHex = CryptoUtils.ByteArrayToHex(privateKey);
            string publicKeyHex = CryptoUtils.ByteArrayToHex(publicKey);
            string msgHashHex = CryptoUtils.ByteArrayToHex(msgHash);
            string signedMessageHex = CryptoUtils.ByteArrayToHex(signedMessage);
            
            return isTestValid;
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
            var info = new MiningBlockInfo {
                Difficulty = appSettings.Difficulty,
                Index = dbService.GetLastBlock().Index + 1,
                MinedBy = address,
                PreviousBlockHash = dbService.GetLastBlock().BlockHash,
                Transactions = new List<Transaction>(dbService.GetTransactions().ToArray())//shallow copy, so we can keep a snapshot
            };

            dbService.Set("block_" + address, info);

            return MiningBlockInfoResponse.FromMiningBlockInfo(info);
        }

        public SubmitBlockResponse SubmitBlockInfo(string address, MinedBlockInfoRequest data)
        {
            var info = dbService.Get<MiningBlockInfo>("block_" + address);
            if (null == info)
            {
                return new SubmitBlockResponse
                {
                    Status = "Error",
                    Message = "Wrong address!"
                };
            }

            var mbi = new MinedBlockInfo(info)
            {
                Nonce = data.Nonce,
                DateCreated = data.DateCreated,
            };

            if (mbi.BlockHash.StartsWith("".PadLeft(appSettings.Difficulty, '0')))
            {
                dbService.AddBlock(mbi);

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
