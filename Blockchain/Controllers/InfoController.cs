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
    [Route("api/info")]
    [Produces("application/json")]
    public class InfoController : BlockchainController
    {
        public InfoController(IBlockchainService blockchainService) : base(blockchainService) { }

        /// <summary>
        /// Gets basic information about the blockchain
        /// </summary>
        [HttpGet]
        public BlockchainInfo Get()
        {
            return _blockchainService.GetBlockchainInfo();
        }
    }
}
