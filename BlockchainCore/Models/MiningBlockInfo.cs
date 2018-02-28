using BlockchainCore.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blockchain.Models
{
    public class MiningBlockInfo
    {
        [JsonProperty(Order = 2)]
        public int Index { get; set; }

        [JsonProperty(Order = 3)]
        public List<Transaction> Transactions { get; set; }

        [JsonProperty(Order = 4)]
        public int Difficulty { get; set; }

        [JsonProperty(Order = 5)]
        public string PreviousBlockHash { get; set; }

        [JsonProperty(Order = 6)]
        public string MinedBy { get; set; }

        [JsonIgnore]
        public string BlockDataHash { get { return CryptoUtils.GetSha256Hex(JsonConvert.SerializeObject(this)); } }

        public static MiningBlockInfo FromMinedBlockInfo(MinedBlockInfo mbi)
        {
            return JsonConvert.DeserializeObject<MiningBlockInfo>(JsonConvert.SerializeObject(mbi));
        }
    }
}
