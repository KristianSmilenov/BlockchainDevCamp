using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    [Serializable]
    public class Transaction
    {
        public string From;
        public string To;
        public decimal Value;
        public string SenderPubKeyHex;
        // SenderSignature: hex_number[2]
        public string[] SenderSignatureHex;
        public string TransactionHashHex;
        public DateTime DateReceived;
        public int MinedInBlockIndex;
        public bool Paid;
    }
}
