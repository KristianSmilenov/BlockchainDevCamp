using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blockchain.Mining;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blockchain.Controllers
{
    [Produces("application/json")]
    [Route("api/mining")]
    public class MiningController : Controller
    {
        protected readonly IMiningService _miningService;

        public MiningController(IMiningService miningService)
        {
            _miningService = miningService ?? throw new ArgumentNullException(nameof(miningService));
        }

        [HttpGet("get-block/{hash}")]
        public MiningBlockInfo Get(string hash)
        {
            return _miningService.GetMiningBlockInfo(hash);
        }
        
        // POST: api/Mining
        [HttpPost("submit-block/{hash}")]
        public SubmitBlockResponse Post(string hash, [FromBody]MinedBlockInfo data)
        {
            return _miningService.SubmitBlockInfo(hash, data);
        }
    }
}
