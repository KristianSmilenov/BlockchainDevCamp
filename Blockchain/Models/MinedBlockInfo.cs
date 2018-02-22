using BlockchainCore.Serializers;
using Newtonsoft.Json;
using System;

namespace Blockchain.Models
{
    public class MinedBlockInfo : MiningBlockInfoHashed
    {
        [JsonProperty(Order = 8)]
        public int Nonce { get; set; }

        [JsonProperty(Order = 9)]
        [JsonConverter(typeof(DateTimeJsonFormatter))]        
        public DateTime DateCreated { get; set; }

        public MinedBlockInfo(MiningBlockInfo blockInfo) : base(blockInfo) { }
    }
}
