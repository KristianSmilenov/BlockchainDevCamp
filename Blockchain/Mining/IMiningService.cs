using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Mining
{
    public interface IMiningService
    {
        MiningBlockInfo GetMiningBlockInfo(string hash);
        SubmitBlockResponse SubmitBlockInfo(string hash, MinedBlockInfo data);
    }
}
