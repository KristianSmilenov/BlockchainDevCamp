using BlockchainCore.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    [DataContract]
    public class MiningBlockInfoResponse : MiningBlockInfo
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public new string BlockDataHash { get; set; }

        [JsonIgnore]
        public new int Transactions { get; set; }

        [JsonIgnore]
        public new int PreviousBlockHash { get; set; }

        [JsonIgnore]
        public new string MinedBy { get; set; }

        public MiningBlockInfoResponse(MiningBlockInfo blockInfo)
        {
            this.Id = blockInfo.Id;
            this.Index = blockInfo.Index;
            this.Difficulty = blockInfo.Difficulty;

            BlockDataHash = CryptoUtils.GetSha256String(JsonConvert.SerializeObject(blockInfo));
        }
    }
}
