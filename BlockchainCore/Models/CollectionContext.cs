using System;
using System.Collections.Generic;
using System.Text;

namespace BlockchainCore.Models
{
    public class CollectionContext<T>
    {
        public List<T> Items { get; set; }
        public int Total { get; set; }
    }
}
