using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebFaucet
{
    public class TransactionModel
    {
        public string NodeUrl { get; set; }
        public string RecipientAddress { get; set; }
        public int Value { get; set; }
    }
}
