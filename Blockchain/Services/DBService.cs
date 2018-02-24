using Blockchain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blockchain.Services
{
    public class DBService : IDBService
    {
        static private Dictionary<string, object> dictionary = new Dictionary<string, object>();
        static private List<MinedBlockInfo> allBlocks = new List<MinedBlockInfo>();
        static private List<Transaction> allTransactions = new List<Transaction>();
        static private List<string> allPeers = new List<string>();
        static private MinedBlockInfo lastBlock;

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
            return allBlocks.Last();
        }

        public List<MinedBlockInfo> GetAllBlocks()
        {
            return allBlocks;
        }

        public void AddBlock(MinedBlockInfo block)
        {
            allBlocks.Add(block);
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

        public List<string> GetPeers()
        {
            return allPeers;
        }

        public void AddPeer(string peerUrl)
        {
            if (!allPeers.Contains(peerUrl))
                allPeers.Add(peerUrl);
        }
    }
}
