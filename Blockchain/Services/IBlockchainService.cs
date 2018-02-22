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
        TransactionHashInfo CreateTransaction(TransactionDataSigned data);
        Transaction GetTransaction(string transactionHash);
        List<string> GetPeers();
        void AddPeer(string peerUrl);

        /*
         * Mining calcuations
         */
        MiningBlockInfo GetMiningBlockInfo(string address);
        SubmitBlockResponse SubmitBlockInfo(string address, MinedBlockInfoRequest data);
    }
}
