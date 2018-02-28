using BlockchainCore.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    public class MinedBlockInfoResponse : MinedBlockInfo
    {
        public new string BlockDataHash { get; set; }
        public new string BlockHash { get; set; }

        public static new MinedBlockInfoResponse FromMinedBlockInfo(MinedBlockInfo mbi)
        {
            var obj = JsonConvert.DeserializeObject<MinedBlockInfoResponse>(JsonConvert.SerializeObject(mbi));
            obj.BlockHash = mbi.BlockHash;

            return obj;
        }
    }
}
