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

        [HttpGet("get-block/{address}")]
        public MiningBlockInfo Get(string address)
        {
            return _blockchainService.GetMiningBlockInfo(address);
        }
        
        [HttpPost("submit-block/{blockId}")]
        public SubmitBlockResponse Post(string blockId, [FromBody]MinedBlockInfo data)
        {
            return _blockchainService.SubmitBlockInfo(blockId, data);
        }
    }
}
