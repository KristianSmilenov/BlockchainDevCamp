using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    [Serializable]
    public class TransactionRequest
    {
        public string From;
        public string To;
        public decimal Amount;
        public string SenderPubKey;
        public string[] SenderSignature;
    }
}
