using BlockchainCore.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blockchain.Models
{
    public class MiningBlockInfo
    {
        private string id = Guid.NewGuid().ToString();

        [JsonProperty(Order = 1)]
        public string Id { get => id; set => id = value; }

        [JsonProperty(Order = 2)]
        public int Index { get; set; }

        [JsonProperty(Order = 3)]
        public List<Transaction> Transactions { get; set; }

        [JsonProperty(Order = 4)]
        public int Difficulty { get; set; }

        [JsonProperty(Order = 5)]
        public string PreviousBlockHash { get; set; }

        [JsonProperty(Order = 6)]
        public string MinedBy { get; set; }
    }
}
