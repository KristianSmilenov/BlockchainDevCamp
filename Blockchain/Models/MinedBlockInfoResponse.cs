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

        public MinedBlockInfoResponse(MinedBlockInfo blockInfo)
        {
            this.BlockDataHash = blockInfo.BlockDataHash;
            this.BlockHash = blockInfo.BlockHash;
        }
    }
}
