using BlockchainCore.Utils;
using Newtonsoft.Json;

namespace Blockchain.Models
{
    public class MinedBlockInfoHashed : MinedBlockInfo
    {
        public string BlockHash { get; set; }

        public MinedBlockInfoHashed(MiningBlockInfoHashed blockInfo) : base(blockInfo)
        {
            BlockHash = CryptoUtils.GetSha256String(JsonConvert.SerializeObject(blockInfo));
        }
    }
}
