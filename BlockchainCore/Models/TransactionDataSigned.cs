using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    [Serializable]
    public class TransactionDataSigned : TransactionData
    {
        public string SenderSignature;
        public string SenderMessage;
    }
}
