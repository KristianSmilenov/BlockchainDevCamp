using Blockchain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebFaucet.Models;

namespace WebFaucet.Services
{
    public interface IDBService
    {
        bool TryGetValue(string key, out TransactionHistory value);
        void Add(string key, TransactionHistory value);
    }
}
