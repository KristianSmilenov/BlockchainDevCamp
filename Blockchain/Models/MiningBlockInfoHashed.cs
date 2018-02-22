using BlockchainCore.Utils;
using Newtonsoft.Json;

namespace Blockchain.Models
{
    public class MiningBlockInfoHashed : MiningBlockInfo
    {
        [JsonProperty(Order = 7)]
        public string BlockDataHash { get; set; }

        public MiningBlockInfoHashed(MiningBlockInfo blockInfo)
        {
            this.Difficulty = blockInfo.Difficulty;
            this.Id = blockInfo.Id;
            this.Index = blockInfo.Index;
            this.MinedBy = blockInfo.MinedBy;
            this.PreviousBlockHash = blockInfo.PreviousBlockHash;
            this.Transactions = blockInfo.Transactions;

            BlockDataHash = CryptoUtils.GetSha256String(JsonConvert.SerializeObject(blockInfo));
        }
    }
}
