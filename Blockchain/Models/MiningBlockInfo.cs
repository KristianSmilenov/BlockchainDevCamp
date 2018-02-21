using Blockchain.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    public class MiningBlockInfo
    {
        private string id = Guid.NewGuid().ToString();

        public string Id { get => id; set => id = value; }

        public int Index { get; set; }

        public int Difficulty { get; set; }
        
        [JsonIgnore]
        public List<Transaction> Transactions { get; set; }

        [JsonIgnore]
        public string PreviousBlockHash { get; set; }

        [JsonIgnore]
        public string MinedBy { get; set; }

        public string BlockDataHash
        {
            get
            {
                var str = "";
                str += Index;
                str += Difficulty;
                str += PreviousBlockHash;
                str += MinedBy;
                str += Transactions != null ? Transactions.Aggregate("", (tr, res) => res + tr.ToString()) : "";

                return CryptoUtils.GetSha256String(str);
            }
        }
    }
}
