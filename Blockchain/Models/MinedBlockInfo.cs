using Blockchain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    public class MinedBlockInfo : MiningBlockInfo
    {
        public int Nonce { get; set; }
        public DateTime DateCreated { get; set; }

        public string BlockHash
        {
            get
            {
                var str = BlockDataHash;
                str += DateCreated;
                str += Nonce;

                return CryptoUtils.GetSha256String(str);
            }
        }
    }
}
