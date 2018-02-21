using Blockchain.Models;
using Blockchain.Services;
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
        [HttpPost("new")]
        public TransactionHashInfo Post([FromBody]TransactionDataSigned data)
        {
            return _blockchainService.CreateTransaction(data);
        }

        /// <summary>
        /// Get transaction info
        /// </summary>
        [HttpGet("{hash}/info")]
        public Transaction Get(string hash)
        {
            return _blockchainService.GetTransaction(hash);
        }
    }
}
