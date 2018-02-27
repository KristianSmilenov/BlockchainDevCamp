using Newtonsoft.Json;
using System;

namespace Blockchain.Models
{
    [Serializable]
    public class Transaction : TransactionDataSigned
    {
        [JsonProperty(PropertyName = "transactionHashHex", Order = 8)]
        public string TransactionHashHex { get; set; }

        [JsonProperty(PropertyName = "minedInBlockIndex", Order = 9)]
        public int MinedInBlockIndex { get; set; }

        [JsonProperty(PropertyName = "transferSuccessful", Order = 10)]
        public bool TransferSuccessful { get; set; }

        public Transaction() { }

        public Transaction(TransactionData data)
        {
            From = data.From;
            To = data.To;
            Value = data.Value;
            Fee = data.Fee;
            DateCreated = data.DateCreated;
            SenderPubKey = data.SenderPubKey;
        }

        public Transaction(TransactionDataSigned data)
            : this((TransactionData)data)
        {
            SenderSignature = data.SenderSignature;
        }
    }
}
