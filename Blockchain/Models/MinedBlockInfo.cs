using BlockchainCore.Serializers;
using BlockchainCore.Utils;
using Newtonsoft.Json;
using System;

namespace Blockchain.Models
{
    public class MinedBlockInfo: MiningBlockInfo
    {
        [JsonProperty(Order = 8)]
        public int Nonce { get; set; }

        [JsonProperty(Order = 9)]
        [JsonConverter(typeof(DateTimeJsonFormatter))]        
        public DateTime DateCreated { get; set; }

        [JsonIgnore]
        public string BlockHash
        {
            get
            {
                return CryptoUtils.GetSha256String(JsonConvert.SerializeObject(this));
            }
        }
    }
}
