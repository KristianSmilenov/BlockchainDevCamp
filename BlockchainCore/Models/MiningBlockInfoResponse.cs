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

        public static MiningBlockInfoResponse FromMiningBlockInfo(MiningBlockInfo blockInfo)
        {
            var obj = JsonConvert.DeserializeObject<MiningBlockInfoResponse>(JsonConvert.SerializeObject(blockInfo));
            obj.BlockDataHash = CryptoUtils.GetSha256Hex(JsonConvert.SerializeObject(blockInfo));

            return obj;
        }
    }
}
