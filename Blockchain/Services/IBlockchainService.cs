using Blockchain.Models;
using BlockchainCore.Models;
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
        List<MinedBlockInfoResponse> GetBlocks();
        MinedBlockInfoResponse GetBlock(int index);
        void NotifyBlock(int index);
        Balance GetBalance(string address);

        /*
         * Transactions
         */
        List<string> GetPeers();
        void AddPeer(string peerUrl);
        PeersNetwork GetPeersNetwork();

        /*
         * Transactions
         */
        List<Transaction> GetPendingTransactions();
        Transaction GetTransaction(string transactionHash);
        TransactionHashInfo CreateTransaction(TransactionDataSigned data);

        /*
         * Mining calcuations
         */
        MiningBlockInfo GetMiningBlockInfo(string address);
        SubmitBlockResponse SubmitBlockInfo(string address, MinedBlockInfoRequest data);
    }
}
