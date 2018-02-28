using Blockchain.Models;
using Blockchain.Services;
using BlockchainCore.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Controllers
{
    [Route("api/transactions")]
    [Produces("application/json")]
    public class TransactionsController : BlockchainController
    {
        public TransactionsController(IBlockchainService blockchainService) : base(blockchainService) { }

        /// <summary>
        /// Create new transaction
        /// </summary>
        [HttpPost]
        public TransactionHashInfo Post([FromBody]TransactionDataSigned data)
        {
            return _blockchainService.CreateTransaction(data);
        }

        /// <summary>
        /// Get transaction info
        /// </summary>
        [HttpGet("{hash}")]
        public IActionResult Get(string hash)
        {
            Transaction result = _blockchainService.GetTransaction(hash);
            if (result != null)
                return Ok(result);

            return NotFound();
        }

        /// <summary>
        /// Get list of transactions
        /// </summary>
        /// <param name="status">Fitler by status: pending and confirmed</param>
        /// <param name="skip">Number of transactions to skip</param>
        /// <param name="take">Number of transactions to take</param>
        [HttpGet()]
        public List<Transaction> GetAll([FromQuery]string status, [FromQuery]int? skip = 0, [FromQuery]int? take = 20)
        {
            return _blockchainService.GetTransactions(status, skip.Value, take.Value);
        }
    }
}
