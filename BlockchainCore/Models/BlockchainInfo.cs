using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    [Serializable]
    public class BlockchainInfo
    {
        public string About;
        public string NodeName;
        public int Peers;
        public int Blocks;
        public int ConfirmedTransactions;
        public int PendingTransactions;
        public int Difficulty;

        public BlockchainInfo(string about, string nodeName)
        {
            About = about;
            NodeName = nodeName;
        }
    }
}
