using Blockchain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Services
{
    public interface IDBService
    {
        object GetTransactionsLockObject();

        T Get<T>(string key);
        void Set(string key, object value);
        void Remove(string key);

        MiningBlockInfo GetMiningInfo(string blockDataHash);
        void AddMiningInfo(MiningBlockInfo info);
        void PurgeMiningInfos(int beforeBlockIndex);

        MinedBlockInfo GetLastBlock();
        List<MinedBlockInfo> GetAllBlocks();
        bool TryAddBlock(MinedBlockInfo block);

        List<Transaction> GetTransactions();
        void AddTransaction(Transaction transaction);
        void RemoveTransaction(Transaction transaction);
        void ClearTransactions();

        List<Peer> GetPeers();
        void AddPeer(Peer peer);
    }
}
