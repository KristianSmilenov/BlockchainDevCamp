using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    public class MiningBlockInfo
    {
        public int Index { get; set; }

        public int Difficulty { get; set; }

        public string BlockDataHash { get; set; }

        [JsonIgnore]
        public List<Transaction> Transactions { get; set; }

        [JsonIgnore]
        public string PreviousBlockHash { get; set; }

        [JsonIgnore]
        public string MinedBy { get; set; }
    }
}
