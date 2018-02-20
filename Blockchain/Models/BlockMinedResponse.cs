using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    public class BlockMinedResponse
    {
        public string Message;
        public string Index;
        public string Transactions;
        public string Proof;
        public string PreviousHash;
    }
}
