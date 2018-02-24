using Blockchain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Services
{
    public interface IDBService
    {
        T Get<T>(string key);
        void Set(string key, object value);
        void Remove(string key);

        MinedBlockInfo GetLastBlock();
        List<MinedBlockInfo> GetAllBlocks();
        void AddBlock(MinedBlockInfo block);

        List<Transaction> GetTransactions();
        void AddTransaction(Transaction transaction);
        void RemoveTransaction(Transaction transaction);
        void ClearTransactions();

        List<string> GetPeers();
        void AddPeer(string peerUrl);
    }
}
