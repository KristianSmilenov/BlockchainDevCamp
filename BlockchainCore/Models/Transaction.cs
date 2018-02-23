using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    [Serializable]
    public class Transaction
    {
        public Transaction() { }

        public Transaction(TransactionDataSigned data)
        {
            From = data.From;
            To = data.To;
            Value = data.Value;
            Fee = data.Fee;
            DateCreated = data.DateCreated;
            SenderPubKey = data.SenderPubKey;
        }

        #region Wallet data
        
        public string From;
        public string To;
        public int Value;
        public int Fee;
        public DateTime DateCreated;
        public string SenderPubKey;

        #endregion

        // TODO: Update to 2 * 64 hex digits (2 * 256 bits)
        public string SenderSignatureHex;
        public string TransactionHashHex;
        public int MinedInBlockIndex;
        public bool TransferSuccessful;
    }
}
