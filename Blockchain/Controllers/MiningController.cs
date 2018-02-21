using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blockchain.Models;
using Blockchain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blockchain.Controllers
{
    [Produces("application/json")]
    [Route("api/mining")]
    public class MiningController : BlockchainController
    {
        public MiningController(IBlockchainService blockchainService) : base(blockchainService) { }

        [HttpGet("get-block/{hash}")]
        public MiningBlockInfo Get(string hash)
        {
            return _blockchainService.GetMiningBlockInfo(hash);
        }
        
        [HttpPost("submit-block/{hash}")]
        public SubmitBlockResponse Post(string hash, [FromBody]MinedBlockInfo data)
        {
            return _blockchainService.SubmitBlockInfo(hash, data);
        }
    }
}
