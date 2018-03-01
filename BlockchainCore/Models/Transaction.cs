using Blockchain.Serializers;
using BlockchainCore.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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

        public Transaction(TransactionDataSigned data)
        {
            From = data.From;
            To = data.To;
            Value = data.Value;
            Fee = data.Fee;
            DateCreated = data.DateCreated;
            SenderPubKey = data.SenderPubKey;
            SenderSignature = data.SenderSignature;
            TransactionHashHex = CalculateTransactionHashHex();
        }
        public string CalculateTransactionHashHex()
        {
            var sett = new JsonSerializerSettings
            {
                ContractResolver = TransactionDataHashReseolver.Instance
            };
            var json = JsonConvert.SerializeObject(this, Formatting.None, sett);

            return CryptoUtils.GetSha256Hex(json);
        }

    }
}
