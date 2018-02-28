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
    [Route("api/blocks")]
    public class BlocksController : BlockchainController
    {
        public BlocksController(IBlockchainService blockchainService) : base(blockchainService) { }

        /// <summary>
        /// Get all blocks in the blockchain
        /// </summary>
        /// <param name="skip">Number of blocks to skip</param>
        /// <param name="take">Number of blocks to take</param>
        [HttpGet]
        public List<MinedBlockInfoResponse> Get([FromQuery]int? skip = 0, [FromQuery]int? take = 20)
        {
            return _blockchainService.GetBlocks(skip.Value, take.Value);
        }

        /// <summary>
        /// Gets specific block by index
        /// </summary>
        [HttpGet("{index}")]
        public IActionResult Get(int index)
        {
            MinedBlockInfoResponse result = _blockchainService.GetBlock(index);
            if (result != null)
                return Ok(result);
            return NotFound();
        }

        /// <summary>
        /// Notifies node about new block
        /// </summary>
        [HttpPost("notify")]
        public void Post([FromBody]NewBlockNotification notification)
        {
            //should we push or pull?
            _blockchainService.NotifyBlock(notification);
        }
    }
}
