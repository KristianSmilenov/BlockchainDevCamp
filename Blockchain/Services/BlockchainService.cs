using Blockchain.Models;
using BlockchainCore.Utils;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;


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
            return new TransactionHashInfo() { IsValid = ValidateTransaction(signedData), DateReceived = DateTime.UtcNow, TransactionHash = "" }  ;
        }

        private bool ValidateTransaction(TransactionDataSigned signedData)
        {
            string testMessage = signedData.SenderMessage; //"Awesome stuff";
            byte[] testMsgHash = CryptoUtils.GetSha256Bytes(testMessage);
            string testPublicKeyHex = signedData.SenderPubKey; // "04fba1bcaf719923609b61e32fcccc9e87a0d87ece58a16e9985b68e06db57987b8ad575979564c498537ce1e12ce9699d51f40730dcd0d151dccd3c501d424301";
            string testSignatureHex = signedData.SenderSignature; //"304402207aa1f7153ad98863f236fe27c85c6ab8a46702ef38f06a08eff37443b7dbdf2d02201f64305fdcf6b74a9cec108dbe630f9f840d8b76c48c810cd0fe7e358ced629f";
            byte[] testPublicKey = CryptoUtils.HexToByteArray(testPublicKeyHex);
            byte[] testSignature = CryptoUtils.HexToByteArray(testSignatureHex);
            bool isTestValid = CryptoUtils.BouncyCastleVerify(testMsgHash, testSignature, testPublicKey);

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
            //1. Create block mining candidate
            //2. Add record to the Node Mining Jobs (address => Block)
            
            var info = new MiningBlockInfo {
                Difficulty = appSettings.Difficulty,
                Index = dbService.GetLastBlock().Index + 1,
                MinedBy = address,
                PreviousBlockHash = dbService.GetLastBlock().BlockHash,
                Transactions = dbService.GetTransactions()
            };

            dbService.Set("block_" + address, info);

            return new MiningBlockInfoResponse(info);
        }

        public SubmitBlockResponse SubmitBlockInfo(string address, MinedBlockInfoRequest data)
        {
            //TODO: Implement
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
