using BlockchainCore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    [Serializable]
    public class TransactionDataSigned : TransactionData
    {
        //[JsonProperty(PropertyName = "senderSignature")]
        //public TransactionSignature SenderSignature;

        [JsonProperty(PropertyName = "senderSignature")]
        public List<string> SenderSignature;
    }
}
