using Blockchain.Models;
using BlockchainCore;
using BlockchainCore.Models;
using BlockchainCore.Utils;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Services
{
    public class BlockchainService : IBlockchainService
    {
        const int FAUCET_START_VOLUME = 1000000;
        const string ZERO_HASH = "0000000000000000000000000000000000000000";
        const string TRANSACTION_API_PATH = "api/transactions";

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
            var tr = new List<Transaction>();
            tr.Add(new Transaction {
                To = appSettings.FaucetAddress,
                From = ZERO_HASH,
                Value = FAUCET_START_VOLUME,
                TransferSuccessful = true,
                DateCreated = DateTime.Now,
                MinedInBlockIndex = 0,
                Fee = 0,
                TransactionHashHex = ZERO_HASH,
            });

            dbService.TryAddBlock(
                new MinedBlockInfo
                {
                    Difficulty = appSettings.Difficulty,
                    Index = 0,
                    MinedBy = "Mr. Bean",
                    PreviousBlockHash = ZERO_HASH,
                    BlockDataHash = ZERO_HASH,
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
            var transaction = dbService.GetTransactions().Find(t => t.TransactionHashHex == transactionHash);
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
            var dateReceived = DateTime.UtcNow;
            var newTransaction = new Transaction(signedData);
            newTransaction.TransactionHashHex = CryptoUtils.GetSha256Hex(JsonConvert.SerializeObject(newTransaction));

            if (dbService.GetTransactions().Any(t => t.TransactionHashHex == newTransaction.TransactionHashHex))
            {
                return new TransactionHashInfo
                {
                    IsValid = true,
                    ErrorMessage = $"Duplicate transaction",
                    DateReceived = dateReceived,
                    TransactionHash = newTransaction.TransactionHashHex
                };
            }

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

            dbService.AddTransaction(newTransaction);
            PropagateTransactionToPeers(newTransaction);

            return new TransactionHashInfo() { IsValid = isValidTransaction, DateReceived = dateReceived, TransactionHash = newTransaction.TransactionHashHex };
        }

        private void PropagateTransactionToPeers(TransactionDataSigned transaction)
        {
            var tasks = new List<Task>();
            dbService.GetPeers().ForEach(p => 
            {
                tasks.Add(Task.Run(() =>
                HttpUtils.DoApiPost<TransactionDataSigned, TransactionHashInfo>(p.Url, TRANSACTION_API_PATH, transaction)));
            });

            Task.WaitAll(tasks.ToArray());
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
            var lastBlockIndex = dbService.GetLastBlock().Index;

            var res = new Balance {
                Address = address,
                ConfirmedBalance = new BalanceInfo { Balance = 0, Confirmations = int.MaxValue },
                LastMinedBalance = new BalanceInfo { Balance = 0, Confirmations = int.MaxValue },
                PendingBalance = new BalanceInfo { Balance = 0, Confirmations = 0 }
            };

            foreach (var block in blocks)
            {
                BalanceInfo bal;
                if (lastBlockIndex - block.Index + 1 >= confirmations)
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
                        bal.Confirmations = Math.Min(bal.Confirmations, lastBlockIndex - block.Index + 1);
                    }
                    else if (val.To == address)
                    {
                        sum += val.Value + val.Fee;
                        bal.Confirmations = Math.Min(bal.Confirmations, lastBlockIndex - block.Index + 1);
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

            res.LastMinedBalance.Balance += res.ConfirmedBalance.Balance;
            res.PendingBalance.Balance += res.LastMinedBalance.Balance;

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
            lock (dbService.GetTransactionsLockObject())
            {
                var transactions = new List<Transaction>(dbService.GetTransactions().ToArray());//shallow copy, so we can keep a snapshot
                var lbi = dbService.GetLastBlock().Index + 1;

                //reward for the miner
                var t = new Transaction()
                {
                    DateCreated = DateTime.Now,
                    Fee = 0,
                    To = address,
                    Value = appSettings.MinerReward + transactions.Aggregate(0, (sum, tr) => sum + tr.Fee),
                    From = ZERO_HASH,
                    MinedInBlockIndex = lbi,
                    TransferSuccessful = true
                };

                t.TransactionHashHex = CryptoUtils.GetSha256Hex(JsonConvert.SerializeObject(t));

                transactions.Insert(0, t);

                var info = new MiningBlockInfo
                {
                    Difficulty = appSettings.Difficulty,
                    Index = lbi,
                    MinedBy = address,
                    PreviousBlockHash = dbService.GetLastBlock().BlockHash,
                    Transactions = transactions
                };

                dbService.AddMiningInfo(info);

                return MiningBlockInfoResponse.FromMiningBlockInfo(info);
            }
        }

        public SubmitBlockResponse SubmitBlockInfo(MinedBlockInfoRequest data)
        {
            lock (dbService.GetTransactionsLockObject())
            {
                var info = dbService.GetMiningInfo(data.BlockDataHash);
                if (null == info)
                {
                    return new SubmitBlockResponse
                    {
                        Status = BlockResponseStatus.Error,
                        Message = "Wrong block hash!"
                    };
                }

                var mbi = new MinedBlockInfo(info)
                {
                    Nonce = data.Nonce,
                    DateCreated = data.DateCreated,
                };

                if (mbi.BlockHash.StartsWith("".PadLeft(appSettings.Difficulty, '0')))
                {
                    var success = dbService.TryAddBlock(mbi);

                    if (!success)
                    {
                        return new SubmitBlockResponse
                        {
                            Status = BlockResponseStatus.Error,
                            Message = "Old block! Someone mined it already"
                        };
                    }

                    //UPDATE transactions in the block.
                    mbi.Transactions.ForEach(t =>
                    {
                        t.MinedInBlockIndex = info.Index;
                        t.TransferSuccessful = true;
                        dbService.RemoveTransaction(t);
                    });

                    PropagateBlockToPeers(mbi);

                    return new SubmitBlockResponse
                    {
                        Status = BlockResponseStatus.Success,
                        Message = $"Block is valid"
                    };
                }
                else
                {
                    return new SubmitBlockResponse
                    {
                        Status = BlockResponseStatus.Error,
                        Message = $"Hash must start with {appSettings.Difficulty} zeroes"
                    };
                }
            }            
        }

        private void PropagateBlockToPeers(MinedBlockInfo block)
        {
            throw new NotImplementedException();
        }
    }
}
