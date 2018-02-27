using BlockchainCore.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    [Serializable]
    public class TransactionData
    {
        [JsonProperty(PropertyName = "from", Order = 1)]
        public string From { get; set; }

        [JsonProperty(PropertyName = "to", Order = 2)]
        public string To { get; set; }

        [JsonProperty(PropertyName = "value", Order = 3)]
        public int Value { get; set; }

        [JsonProperty(PropertyName = "fee", Order = 4)]
        public int Fee { get; set; }

        [JsonConverter(typeof(DateTimeJsonFormatter))]
        [JsonProperty(PropertyName = "dateCreated", Order = 5)]
        public DateTime DateCreated { get; set; }

        [JsonProperty(PropertyName = "senderPubKey", Order = 6)]
        public string SenderPubKey { get; set; }

        public TransactionData() { }

        public TransactionData(TransactionDataSigned data)
        {
            From = data.From;
            To = data.To;
            Value = data.Value;
            Fee = data.Fee;
            SenderPubKey = data.SenderPubKey;
            DateCreated = data.DateCreated;
        }
    }
}
