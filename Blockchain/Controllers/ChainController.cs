using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blockchain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blockchain.Controllers
{
    [Route("api/chain")]
    [Produces("application/json")]
    public class ChainController : Controller
    {
        [HttpGet]
        public ChainResponse Get()
        {
            return new ChainResponse();
        }
    }
}
