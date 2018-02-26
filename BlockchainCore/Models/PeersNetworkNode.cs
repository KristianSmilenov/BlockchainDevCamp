using Newtonsoft.Json;
using System;

namespace BlockchainCore.Models
{
    [Serializable]
    public class PeersNetworkNode
    {
        public string Id { get; set; }
        public String Label { get; set; }
    }
}
