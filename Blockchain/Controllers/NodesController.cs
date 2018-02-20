using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blockchain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blockchain.Controllers
{
    [Route("api/nodes")]
    [Produces("application/json")]
    public class NodesController : Controller
    {
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