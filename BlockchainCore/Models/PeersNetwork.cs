using Newtonsoft.Json;
using System.Collections.Generic;

namespace BlockchainCore.Models
{
    public class PeersNetwork
    {
        [JsonProperty(PropertyName = "nodes")]
        public List<PeersNetworkNode> Nodes;
        [JsonProperty(PropertyName = "edges")]
        public List<PeersNetworkEdge> Edges;
    }
}
