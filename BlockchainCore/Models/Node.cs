using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    [Serializable]
    public class Node
    {
        public List<string> Peers;
        public List<Block> Blocks;
        public List<Transaction> PendingTransactions;
        public Dictionary<string, int> Balances;
        public int Difficulty;
        public Dictionary<string, Block> MiningJobs;
    }
}