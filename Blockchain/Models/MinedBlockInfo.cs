using BlockchainCore.Utils;
using System;

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
