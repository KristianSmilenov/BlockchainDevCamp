using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    public class MinedBlockInfo
    {
        public int Nonce;
        public DateTime DateCreated;
        public string BlockHash;
    }
}