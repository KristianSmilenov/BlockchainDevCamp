using Blockchain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Services
{
    public interface IDBService
    {
        object Get(string key);
        void Set(string key, object value);
        void Remove(string key);

        MinedBlockInfoHashed GetLastBlock();
        void SetLastBlock(MinedBlockInfoHashed block);

        List<Transaction> GetTransactions();
        void AddTransaction(Transaction transaction);
        void RemoveTransaction(Transaction transaction);
        void ClearTransactions();
    }
}
