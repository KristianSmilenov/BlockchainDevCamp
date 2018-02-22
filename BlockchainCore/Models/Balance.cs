using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    [Serializable]
    public class Balance
    {
        public string Address;
        public BalanceInfo ConfirmedBalance;
        public BalanceInfo LastMinedBalance;
        public BalanceInfo PendingBalance;
    }
}
