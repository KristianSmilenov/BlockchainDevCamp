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
    [Route("api/nodes")]
    [Produces("application/json")]
    public class NodesController : BlockchainController
    {
        public NodesController(IBlockchainService blockchainService) : base(blockchainService) { }

        [HttpGet]
        [Route("/resolve")]
        public ResolvedChainResponse Get()
        {
            return new ResolvedChainResponse();
        }

        [HttpPost]
        [Route("/register")]
        public void Post([FromBody]RegisterNodeRequest data)
        {

        }
    }
}