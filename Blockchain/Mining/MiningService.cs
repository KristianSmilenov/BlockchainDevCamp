using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Mining
{
    public class MiningService : IMiningService
    {
        public MiningBlockInfo GetMiningBlockInfo(string hash)
        {
            //TODO: Implement
            return new MiningBlockInfo();
        }

        public SubmitBlockResponse SubmitBlockInfo(string hash, MinedBlockInfo data)
        {
            //TODO: Implement
            return new SubmitBlockResponse();
        }
    }
}
