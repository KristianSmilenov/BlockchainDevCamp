using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Services
{
    public interface IDBService
    {
        object Get(string key);
        void Set(string key, object value);
    }
}
