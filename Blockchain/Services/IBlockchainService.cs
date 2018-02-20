using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Services
{
    public interface IBlockchainService
    {
        void RegisterNode(string name);
        void ValidateChain();
        void ResolveConflicts();
        void CreateBlock();
        void CreateTransaction();
    }
}
