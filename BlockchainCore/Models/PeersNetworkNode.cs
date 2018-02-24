using Newtonsoft.Json;
using System;

namespace BlockchainCore.Models
{
    [Serializable]
    public class PeersNetworkNode
    {
        [JsonProperty(PropertyName = "id")]
        public int Id;
        [JsonProperty(PropertyName = "label")]
        public String Label;
    }
}
