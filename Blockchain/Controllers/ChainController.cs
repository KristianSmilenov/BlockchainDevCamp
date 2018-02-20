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
    [Route("api/chain")]
    [Produces("application/json")]
    public class ChainController : BlockchainController
    {
        public ChainController(IBlockchainService blockchainService) : base(blockchainService) { }

        [HttpGet]
        public ChainResponse Get()
        {
            return new ChainResponse();
        }
    }
}
