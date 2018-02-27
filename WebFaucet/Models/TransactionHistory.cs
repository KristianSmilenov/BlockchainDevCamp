using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebFaucet.Models
{
    public class TransactionHistory
    {
        public string Address { get; set; }
        public DateTime DateReceived { get; set; }
        public int Amount { get; set; }
    }
}
