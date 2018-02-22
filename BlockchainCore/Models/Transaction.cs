using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    [Serializable]
    public class Transaction
    {
        #region Wallet data
        
        // From address should always match the sender public key
        public string From;

        // 40 hex digits
        public string To;
        public decimal Value;
        public decimal Fee;

        // ISO8601 UTC datetime string
        public DateTime DateCreated;

        // 65 hex digits
        public string SenderPubKeyHex;

        #endregion

        // ECDSA signature consists of 2 * 64 hex digits (2 * 256 bits)
        public string[] SenderSignatureHex;
        public string TransactionHashHex;
        public int MinedInBlockIndex;
        public bool TransferSuccessful;
    }
}
