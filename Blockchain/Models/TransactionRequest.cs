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
        public string SenderPubKey;
        public decimal Value;
        public decimal Fee;
        public DateTime DateCreated;
        public string[] SenderSignature;
    }
}
