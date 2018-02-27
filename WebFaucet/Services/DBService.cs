using Blockchain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using WebFaucet.Models;

namespace WebFaucet.Services
{
    public class DBService : IDBService
    {
        private Dictionary<string, TransactionHistory> faucetDonationHistory = new Dictionary<string, TransactionHistory>();

        public bool TryGetValue(string key, out TransactionHistory value)
        {
            return faucetDonationHistory.TryGetValue(key, out value);
        }

        public void Add(string key, TransactionHistory value)
        {
            faucetDonationHistory[key] = value;
        }
    }
}
