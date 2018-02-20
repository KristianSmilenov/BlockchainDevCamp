using Blockchain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Services
{
    public class BlockchainService : IBlockchainService
    {
        private BlockchainInfo info;
        private List<Block> blocks;

        public BlockchainService()
        {
            info = new BlockchainInfo("Overwatch Blockchain", "Genesis");
            blocks = new List<Block>();
        }

        public BlockchainInfo GetBlockchainInfo()
        {
            return info;
        }
        
        public List<Block> GetBlocks()
        {
            return blocks;
        }
        
        public Block GetBlock(int index)
        {
            if(blocks.Count + 1 < index)
                return null;
            return blocks.ElementAt(index);
        }
        
        public void NotifyBlock(int index)
        {
            throw new NotImplementedException();
        }

        public TransactionHashInfo CreateTransaction(TransactionRequest data)
        {
            //TODO: Implement
            return new TransactionHashInfo();
        }

        public Balance GetBalance(string address)
        {
            //TODO: Implement
            return new Balance();
        }

        public Transaction GetTransaction(string transactionHash)
        {
            //TODO: Implement
            return new Transaction();
        }

        public List<string> GetPeers()
        {
            //TODO: Implement
            return new List<string>();
        }

        public void AddPeer(string peerUrl)
        {
            //TODO: Implement
        }

        public void CreateBlock()
        {
            throw new NotImplementedException();
        }
        
        public void ResolveConflicts()
        {
            throw new NotImplementedException();
        }

        public void ValidateChain()
        {
            throw new NotImplementedException();
        }
    }
}
