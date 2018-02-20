using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    [Serializable]
    public class BalanceInfo
    {
        public int Confirmations;
        public decimal Balance;
    }
}
