using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blockchain.Models;
using Blockchain.Services;
using BlockchainCore.Models;
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
        [HttpGet]
        public CollectionContext<MinedBlockInfoResponse> Get([FromQuery]int? pageNumber = 0, [FromQuery]int? pageSize = 20)
        {
            return _blockchainService.GetBlocksCollection(pageNumber.Value, pageSize.Value);
        }

        /// <summary>
        /// Get all blocks in the blockchain
        /// </summary>
        [HttpGet("sync")]
        public List<MinedBlockInfoResponse> GetSyncBlocks()
        {
            return _blockchainService.GetBlocks();
        }


        /// <summary>
        /// Gets specific block by index
        /// </summary>
        [HttpGet("index/{index}")]
        public IActionResult Get(int index)
        {
            MinedBlockInfoResponse result = _blockchainService.GetBlock(index);
            if (result != null)
                return Ok(result);
            return NotFound();
        }

        /// <summary>
        /// Gets specific block by hash
        /// </summary>
        [HttpGet("hash/{hash}")]
        public IActionResult Get(string hash)
        {
            MinedBlockInfoResponse result = _blockchainService.GetBlock(hash);
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
