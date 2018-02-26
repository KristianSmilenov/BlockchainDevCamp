using Newtonsoft.Json;
using System;

namespace BlockchainCore.Models
{
    [Serializable]
    public class PeersNetworkEdge
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}