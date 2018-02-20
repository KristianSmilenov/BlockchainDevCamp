using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Mining
{
    public class MiningBlockInfo
    {
        public int Index;
        public int TransactionsIncluded;
        public decimal ExpectedReward;
        public string BlockHash;
    }
}
