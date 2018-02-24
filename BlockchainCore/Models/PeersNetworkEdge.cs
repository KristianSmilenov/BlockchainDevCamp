using Newtonsoft.Json;
using System;

namespace BlockchainCore.Models
{
    [Serializable]
    public class PeersNetworkEdge
    {
        [JsonProperty(PropertyName = "from")]
        public int From;
        [JsonProperty(PropertyName = "to")]
        public int To;
    }
}