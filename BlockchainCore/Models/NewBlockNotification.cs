using Blockchain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.Models
{
    public class NewBlockNotification
    {
        public MinedBlockInfo LastBlock { get; set; }
        public Peer Sender { get; set; }
    }
}
