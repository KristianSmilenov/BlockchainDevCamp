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
        static private MinedBlockInfo lastBlock;
        static private List<Transaction> transactions = new List<Transaction>();

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
            return transactions;
        }

        public void AddTransaction(Transaction transaction)
        {
            transactions.Add(transaction);
        }

        public void RemoveTransaction(Transaction transaction)
        {
            transactions.Remove(transaction);
        }

        public void ClearTransactions()
        {
            transactions.Clear();
        }
    }
}
