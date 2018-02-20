using Blockchain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Services
{
    public interface IBlockchainService
    {
        BlockchainInfo GetBlockchainInfo();

        /*
         * Blockchain Node
         */
        List<Block> GetBlocks();
        Block GetBlock(int index);
        void NotifyBlock(int index);
        Balance GetBalance(string address);
        TransactionHashInfo CreateTransaction(TransactionRequest data);
        Transaction GetTransaction(string transactionHash);
        List<string> GetPeers();
        void AddPeer(string peerUrl);


        void ValidateChain();
        void ResolveConflicts();
        void CreateBlock();
    }
}
