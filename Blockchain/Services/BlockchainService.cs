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
        private Peer thisPeer = new Peer {
            Id = Guid.NewGuid().ToString(),
        };

        public BlockchainService(IDBService dbService, IOptions<AppSettings> appSettings)
        {
            this.dbService = dbService;
            this.appSettings = appSettings.Value;
            thisPeer.Url = appSettings.Value.NodeUrl;
            thisPeer.Name = appSettings.Value.NodeName;

            GenerateGenesisBlock();
        }

        private void GenerateGenesisBlock()
        {
            //TODO: Give faucet some coins in the Genesis block. How? How do we know the faucet's address?
            var tr = new List<Transaction>();
            tr.Add(new Transaction {
                To = appSettings.FaucetAddress,
                Value = 100000,
                TransferSuccessful = true,
                DateCreated = DateTime.Now,
                MinedInBlockIndex = 0                
            });

            dbService.AddBlock(
                new MinedBlockInfo
                {
                    Difficulty = appSettings.Difficulty,
                    Index = 0,
                    MinedBy = "Mr. Bean",
                    PreviousBlockHash = "",
                    Transactions = tr,
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
            var transaction = dbService.GetTransactions().Find(t => t.TransactionHashHex.Equals(transactionHash));
            if(transaction != null)
            {
                return transaction;
            }

            foreach (var b in dbService.GetAllBlocks())
            {
                var res = b.Transactions.FirstOrDefault(t => t.TransactionHashHex == transactionHash);
                if (res != null)
                    return res;
            }

            return null;
        }
        
        public List<Transaction> GetTransactions(string status)
        {
            switch (status)
            {
                case "confirmed":
                    return dbService
                        .GetAllBlocks()
                        .Aggregate(new List<Transaction>(), (acc, b) => 
                        {
                            acc.AddRange(b.Transactions);
                            return acc;
                        });

                case "pending":
                default:
                    return dbService.GetTransactions().ToList();
            }
        }

        public TransactionHashInfo CreateTransaction(TransactionDataSigned signedData)
        {
            //TODO: Make all fields required

            DateTime dateReceived = DateTime.UtcNow;
            bool isValidTransaction = ValidateTransaction(signedData);

            if (!isValidTransaction)
            {
                return new TransactionHashInfo
                {
                    IsValid = isValidTransaction,
                    ErrorMessage = "Transaction data is corrupted.",
                    DateReceived = dateReceived,
                    TransactionHash = ""
                };
            }

            //validate balance
            var bal = GetBalance(signedData.From, 1).ConfirmedBalance.Balance;
            var hasFunds = (bal >= signedData.Value + signedData.Fee);

            if (!hasFunds)
            {
                return new TransactionHashInfo
                {
                    IsValid = isValidTransaction,
                    ErrorMessage = $"Not enough funds. Available funds: {bal}, required funds: {signedData.Value} + {signedData.Fee} for the fee",
                    DateReceived = dateReceived,
                    TransactionHash = ""
                };
            }

            var newTransaction = new Transaction(signedData);
            newTransaction.SenderSignatureHex = signedData.SenderSignature;
            newTransaction.TransactionHashHex = CryptoUtils.GetSha256Hex(JsonConvert.SerializeObject(newTransaction));
            dbService.AddTransaction(newTransaction);
            return new TransactionHashInfo() { IsValid = isValidTransaction, DateReceived = dateReceived, TransactionHash = newTransaction.TransactionHashHex };

            //TODO: Send transaction to Peers
        }

        private bool ValidateTransaction(TransactionDataSigned signedData)
        {
            TransactionData toValidate = new TransactionData(signedData);
            var messageData = JsonConvert.SerializeObject(toValidate);

            // Validate signature
            var senderPublicKey = CryptoUtils.HexToByteArray(signedData.SenderPubKey);
            var senderSignature = CryptoUtils.HexToByteArray(signedData.SenderSignature);
            var messageDataHash = CryptoUtils.GetSha256Bytes(messageData);
            var isTestValid = CryptoUtils.BouncyCastleVerify(messageDataHash, senderSignature, senderPublicKey);

            return isTestValid;
        }

        public Balance GetBalance(string address, int confirmations)
        {
            var blocks = dbService.GetAllBlocks();
            var lastBlockIndex = blocks.Last().Index;

            var res = new Balance {
                Address = address,
                ConfirmedBalance = new BalanceInfo { Balance = 0, Confirmations = 0},
                LastMinedBalance = new BalanceInfo { Balance = 0, Confirmations = 0 },
                PendingBalance = new BalanceInfo { Balance = 0, Confirmations = 0 }
            };

            foreach (var block in blocks)
            {
                BalanceInfo bal;
                if (lastBlockIndex - confirmations <= block.Index + 1)
                {
                    bal = res.ConfirmedBalance;
                }
                else {
                    bal = res.LastMinedBalance;
                }

                bal.Balance += block.Transactions.Aggregate(0, (sum, val) =>
                {
                    if (val.From == address)
                    {
                        sum -= val.Value + val.Fee;
                        bal.Confirmations = lastBlockIndex - block.Index;
                    }
                    else if (val.To == address)
                    {
                        sum += val.Value + val.Fee;
                        bal.Confirmations = lastBlockIndex - block.Index;
                    }

                    return sum;
                });
            }

            res.PendingBalance.Balance = dbService.GetTransactions().Aggregate(0, (sum, val) =>
            {
                if (val.From == address)
                {
                    sum -= val.Value + val.Fee;
                }
                else if (val.To == address)
                {
                    sum += val.Value + val.Fee;
                }

                return sum;
            });

            return res;
        }

        public List<Peer> GetPeers()
        {
            return dbService.GetPeers();
        }

        public void AddPeer(Peer peer)
        {
            dbService.AddPeer(peer);
        }

        public PeersNetwork GetPeersNetwork()
        {
            var network = new PeersNetwork
            {
                Nodes = new List<PeersNetworkNode>(),
                Edges = new List<PeersNetworkEdge>()
            };

            network.Nodes.Add(new PeersNetworkNode { Id = thisPeer.Id, Label = thisPeer.Name });

            foreach (var p in dbService.GetPeers())
            {
                network.Nodes.Add(new PeersNetworkNode {Id = p.Id, Label = p.Name });
                network.Edges.Add(new PeersNetworkEdge {From = p.Id, To = thisPeer.Id });
            }

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
