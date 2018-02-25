using Blockchain.Models;
using BlockchainCore.Models;
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
        private IDBService dbService;
        private AppSettings appSettings;

        public BlockchainService(IDBService dbService, IOptions<AppSettings> appSettings)
        {
            this.dbService = dbService;
            this.appSettings = appSettings.Value;

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
            var info = new BlockchainInfo("B-Chain", "MainNode");
            info.Difficulty = appSettings.Difficulty;
            info.Peers = dbService.GetPeers().Count;
            info.PendingTransactions = dbService.GetTransactions().Count;
            info.Blocks = dbService.GetAllBlocks().Count;
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

        public Transaction GetTransaction(string transactionHash)
        {
            Transaction transaction = dbService.GetTransactions().Find(t => t.TransactionHashHex.Equals(transactionHash));
            if(transaction == null)
            {
                //TODO: Iterate all blocks and look for the transcation
            }

            return transaction;
        }
        
        public List<Transaction> GetTransactions(string status)
        {
            List<Transaction> pending = dbService.GetTransactions().OrderByDescending(t => t.DateCreated).ToList();
            if (!String.IsNullOrEmpty(status))
            {
                if(status.Equals("pending"))
                    return pending;
                if (status.Equals("confirmed"))
                    //TODO: Implement retrieval of confirmed transactions
                    return new List<Transaction>();
            }
            return pending;
        }

        public TransactionHashInfo CreateTransaction(TransactionDataSigned signedData)
        {
            //TODO: Make all fields required

            DateTime dateReceived = DateTime.UtcNow;
            bool isValidTransaction = ValidateTransaction(signedData);
            if (isValidTransaction)
            {
                Transaction newTransaction = new Transaction(signedData);
                newTransaction.SenderSignatureHex = signedData.SenderSignature;
                newTransaction.TransactionHashHex = GetTransactionHash(newTransaction);
                dbService.GetTransactions().Add(newTransaction);
                return new TransactionHashInfo() { IsValid = isValidTransaction, DateReceived = dateReceived, TransactionHash = newTransaction.TransactionHashHex };
            }

            //TODO: Send transaction to Peers
            return new TransactionHashInfo() { IsValid = isValidTransaction, ErrorMessage = "Transaction data is corrupted.", DateReceived = dateReceived, TransactionHash = "" };
        }

        private string GetTransactionHash(Transaction transaction)
        {
            return CryptoUtils.GetSha256Hex(JsonConvert.SerializeObject(transaction));
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
            return null;
        }

        public List<string> GetPeers()
        {
            return dbService.GetPeers();
        }

        public void AddPeer(string peerUrl)
        {
            dbService.AddPeer(peerUrl);
        }

        public PeersNetwork GetPeersNetwork()
        {
            //TODO: Implement
            return GetMockedPeersNetwork();
        }

        private PeersNetwork GetMockedPeersNetwork()
        {
            PeersNetwork network = new PeersNetwork();
            List<PeersNetworkNode> nodes = new List<PeersNetworkNode>()
            {
                new PeersNetworkNode() {Id=1, Label="Node 1" },
                new PeersNetworkNode() {Id=2, Label="Node 2" },
                new PeersNetworkNode() {Id=3, Label="Node 3" },
                new PeersNetworkNode() {Id=4, Label="Node 4" },
                new PeersNetworkNode() {Id=5, Label="Node 5" },
                new PeersNetworkNode() {Id=6, Label="Node 6" }
            };
            network.Nodes = nodes;

            List<PeersNetworkEdge> edges = new List<PeersNetworkEdge>()
            {
                new PeersNetworkEdge() { From=1, To=2},
                new PeersNetworkEdge() { From=2, To=4},
                new PeersNetworkEdge() { From=2, To=5},
                new PeersNetworkEdge() { From=3, To=5},
                new PeersNetworkEdge() { From=5, To=6}
            };
            network.Edges = edges;
            return network;
        }

        public MiningBlockInfo GetMiningBlockInfo(string address)
        {
            var info = new MiningBlockInfo
            {
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
