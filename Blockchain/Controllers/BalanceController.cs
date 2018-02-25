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
    [Route("api/balance")]
    public class BalanceController : BlockchainController
    {
        public BalanceController(IBlockchainService blockchainService) : base(blockchainService) { }

        /// <summary>
        /// Get address balance
        /// </summary>
        [HttpGet("{address}")]
        public IActionResult Get(string address)
        {
            Balance result = _blockchainService.GetBalance(address);
            if (result != null)
                return Ok(result);
            return NotFound();
        }
    }
}
