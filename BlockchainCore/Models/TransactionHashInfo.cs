using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    [Serializable]
    public class TransactionHashInfo
    {
        public bool IsValid;
        public DateTime DateReceived;
        public string TransactionHash;
    }
}
