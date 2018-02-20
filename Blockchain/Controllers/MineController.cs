using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blockchain.Models;
using Blockchain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blockchain.Controllers
{
    [Route("api/mine")]
    [Produces("application/json")]
    public class MineController : BlockchainController
    {
        public MineController(IBlockchainService blockchainService) : base(blockchainService) { }

        /// <summary>
        /// Mines a new block in the blockchain
        /// </summary>
        [HttpGet]
        public BlockMinedResponse Get()
        {
            /*
             * 1. We run the proof of work algorithm to get the next proof
             * 2. We must receive a reward for finding the proof
             * 3. Forge the new Block by adding it to the chain
             */

            return new BlockMinedResponse();
        }
    }
}
