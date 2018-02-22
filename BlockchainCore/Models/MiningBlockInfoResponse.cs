using BlockchainCore.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    public class MiningBlockInfoResponse : MiningBlockInfo
    {
        [JsonProperty(Order = 7)]
        public new string BlockDataHash { get; set; }

        public MiningBlockInfoResponse() { }

        public MiningBlockInfoResponse(MiningBlockInfo blockInfo)
        {
            this.Index = blockInfo.Index;
            this.Difficulty = blockInfo.Difficulty;

            BlockDataHash = CryptoUtils.GetSha256Hex(JsonConvert.SerializeObject(blockInfo));
        }
    }
}
