using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    [Serializable]
    public class TransactionData
    {
        public string From;
        public string To;
        public string SenderPubKey;
        public decimal Value;
        public decimal Fee;
        //public DateTime DateCreated;

        public TransactionData() { }

        public TransactionData(TransactionDataSigned data)
        {
            From = data.From;
            To = data.To;
            SenderPubKey = data.SenderPubKey;
            Value = data.Value;
            Fee = data.Fee;
            //DateCreated = data.DateCreated;
        }
    }
}
