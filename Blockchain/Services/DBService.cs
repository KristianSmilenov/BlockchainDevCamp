using Blockchain.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Blockchain.Services
{
    public class DBService : IDBService
    {
        static private Dictionary<string, object> dictionary = new Dictionary<string, object>();
        static private ConcurrentDictionary<int, MinedBlockInfo> allBlocks = new ConcurrentDictionary<int, MinedBlockInfo>();
        static private List<Transaction> allTransactions = new List<Transaction>();
        static private List<Peer> allPeers = new List<Peer>();
        static private Dictionary<string, MiningBlockInfo> allBlockInfos = new Dictionary<string, MiningBlockInfo>();

        public object GetTransactionsLockObject()
        {
            return allTransactions;
        }

        public T Get<T>(string key)
        {
            return (T)dictionary.GetValueOrDefault(key);
        }

        public void Set(string key, object value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        public void Remove(string key)
        {
            dictionary.Remove(key);
        }

        public MinedBlockInfo GetLastBlock()
        {
            var keys = allBlocks.Keys.ToList();
            keys.Sort();
            return allBlocks[keys.Last()];
        }

        public List<MinedBlockInfo> GetAllBlocks()
        {
            return allBlocks.Values.ToList();
        }

        public bool TryAddBlock(MinedBlockInfo block)
        {
            return allBlocks.TryAdd(block.Index, block);
        }

        public List<Transaction> GetTransactions()
        {
            return allTransactions;
        }

        public void AddTransaction(Transaction transaction)
        {
            allTransactions.Add(transaction);
        }

        public void RemoveTransaction(Transaction transaction)
        {
            allTransactions.Remove(transaction);
        }

        public void ClearTransactions()
        {
            allTransactions.Clear();
        }

        public List<Peer> GetPeers()
        {
            return allPeers;
        }

        public void AddPeer(Peer peer)
        {
            if (!allPeers.Contains(peer))
                allPeers.Add(peer);
        }

        public MiningBlockInfo GetMiningInfo(string blockDataHash)
        {
            return allBlockInfos.GetValueOrDefault(blockDataHash);
        }

        public void AddMiningInfo(MiningBlockInfo info)
        {
            allBlockInfos.TryAdd(info.BlockDataHash, info);
        }

        public void PurgeMiningInfos(int beforeBlockIndex)
        {
            allBlockInfos
                .Where(b => b.Value.Index < beforeBlockIndex).ToList()
                .ForEach(i => allBlockInfos.Remove(i.Key));
        }
    }
}
