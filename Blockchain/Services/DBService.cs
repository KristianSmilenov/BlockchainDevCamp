using System;
using System.Collections.Generic;
using System.Linq;

namespace Blockchain.Services
{
    public class DBService : IDBService
    {
        static Dictionary<string, object> dictionary = new Dictionary<string, object>();

        public object Get(string key)
        {
            if (key == "asd") return "ASD";
            return dictionary.GetValueOrDefault(key);
        }

        public void Set(string key, object value)
        {
            dictionary.Add(key, value);
        }
    }
}
