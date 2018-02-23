using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Org.BouncyCastle.Math;
using System.Text;

namespace BlockchainCore.Models
{
    [Serializable]
    public class TransactionSignature
    {
        [JsonProperty(PropertyName = "r")]
        public BigInteger R;
        [JsonProperty(PropertyName = "recoveryParams")]
        public int RecoveryParams;
        [JsonProperty(PropertyName = "s")]
        public BigInteger S;
    }
}
