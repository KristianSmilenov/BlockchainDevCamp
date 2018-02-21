using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    [Serializable]
    public class Block
    {
        #region Block template info

        public int Index;
        public List<Transaction> Transactions;
        public int Difficulty;
        public string PrevBlockHashHex;
        public string MinedBy;
        public string BlockDataHash;

        #endregion

        #region Provided by the miner

        public int Nonce;
        public DateTime DateCreated;
        public string BlockHashHex;

        #endregion
    }
}