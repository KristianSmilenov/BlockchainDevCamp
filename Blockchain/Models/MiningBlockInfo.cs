using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    public class MiningBlockInfo
    {
        public int Index;
        public int TransactionsIncluded;
        public decimal ExpectedReward;
        public string BlockHash;
    }
}
