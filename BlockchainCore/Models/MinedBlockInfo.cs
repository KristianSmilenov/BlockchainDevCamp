using BlockchainCore.Serializers;
using BlockchainCore.Utils;
using Newtonsoft.Json;
using System;

namespace Blockchain.Models
{
    public class MinedBlockInfo : MiningBlockInfo
    {
        [JsonProperty(Order = 8)]
        public ulong Nonce { get; set; }

        [JsonProperty(Order = 9)]
        [JsonConverter(typeof(DateTimeJsonFormatter))]
        public DateTime DateCreated { get; set; }
        
        public new string BlockDataHash { get; set; }

        [JsonIgnore]
        public string BlockHash
        {
            get
            {
                return CryptoUtils.GetSha256Hex(this.BlockDataHash + this.DateCreated.ToUniversalTime().ToString("o") + this.Nonce);
            }
        }

        public MinedBlockInfo() { }

        public MinedBlockInfo(MiningBlockInfo mbi)
        {
            this.Transactions = mbi.Transactions;
            this.PreviousBlockHash = mbi.PreviousBlockHash;
            this.MinedBy = mbi.MinedBy;
            this.Index = mbi.Index;
            this.Difficulty = mbi.Difficulty;
            this.BlockDataHash = mbi.BlockDataHash;
        }
    }
}
