using Blockchain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blockchain.Services
{
    public class DBService : IDBService
    {
        static private Dictionary<string, object> dictionary = new Dictionary<string, object>();

        static private MinedBlockInfoHashed lastBlock;
        static private List<Transaction> transactions = new List<Transaction>();

        public object Get(string key)
        {
            return dictionary.GetValueOrDefault(key);
        }

        public void Set(string key, object value)
        {
            dictionary.Add(key, value);
        }

        public void Remove(string key)
        {
            dictionary.Remove(key);
        }

        public MinedBlockInfoHashed GetLastBlock()
        {
            return lastBlock;
        }

        public void SetLastBlock(MinedBlockInfoHashed block)
        {
            lastBlock = block;
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
