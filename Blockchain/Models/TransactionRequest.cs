using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    public class TransactionRequest
    {
        public string Sender;
        public string Recipient;
        public decimal Amount;
    }
}
