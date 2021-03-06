﻿using Blockchain.Models;
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
        CollectionContext<MinedBlockInfoResponse> GetBlocksCollection(int pageNumber, int pageSize);
        List<MinedBlockInfoResponse> GetBlocks();
        MinedBlockInfoResponse GetBlock(int index);
        MinedBlockInfoResponse GetBlock(string blockHash);
        void NotifyBlock(NewBlockNotification notification);
        Balance GetBalance(string address, int confirmations);

        /*
         * Transactions
         */
        List<Peer> GetPeers();
        void AddPeer(Peer peer);
        PeersNetwork GetPeersNetwork();

        /*
         * Transactions
         */
        Transaction GetTransaction(string transactionHash);
        CollectionContext<Transaction> GetTransactions(string status, int pageNumber, int pageSize);
        //List<Transaction> GetTransactions(string status, int skip, int take);
        TransactionHashInfo CreateTransaction(TransactionDataSigned data);

        /*
         * Mining calcuations
         */
        MiningBlockInfo GetMiningBlockInfo(string address);
        SubmitBlockResponse SubmitBlockInfo(MinedBlockInfoRequest data);
    }
}
