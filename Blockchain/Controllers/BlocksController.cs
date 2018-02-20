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
        [HttpGet]
        public List<Block> Get()
        {
            return _blockchainService.GetBlocks();
        }

        /// <summary>
        /// Gets specific block by index
        /// </summary>
        [HttpGet("/{index}", Name = "Get")]
        public Block Get(int index)
        {
            return _blockchainService.GetBlock(index);
        }

        /// <summary>
        /// Notifies node about new block
        /// </summary>
        [HttpPost]
        [Route("/notify")]
        public void Post([FromBody]int index)
        {
            _blockchainService.NotifyBlock(index);
        }
    }
}
