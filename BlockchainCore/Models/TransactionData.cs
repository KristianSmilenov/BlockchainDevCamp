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
        [JsonProperty(PropertyName = "from")]
        public string From;

        [JsonProperty(PropertyName = "to")]
        public string To;

        [JsonProperty(PropertyName = "value")]
        public int Value;

        [JsonProperty(PropertyName = "fee")]
        public int Fee;

        [JsonProperty(PropertyName = "senderPubKey")]
        public string SenderPubKey;

        [JsonConverter(typeof(DateTimeJsonFormatter))]
        [JsonProperty(PropertyName = "dateCreated")]
        public DateTime DateCreated;

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
