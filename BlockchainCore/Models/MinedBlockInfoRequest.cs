using BlockchainCore.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.Models
{
    public class MinedBlockInfoRequest
    {
        public ulong Nonce { get; set; }

        [JsonConverter(typeof(DateTimeJsonFormatter))]
        public DateTime DateCreated { get; set; }
    }
}
