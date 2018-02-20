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

        [HttpPost]
        [Route("/new")]
        public TransactionHashInfo Post([FromBody]TransactionRequest data)
        {
            return _blockchainService.CreateTransaction(data);
        }

        [HttpGet("/{transactionHash}/info", Name = "Get")]
        public Transaction Get(string transactionHash)
        {
            return _blockchainService.GetTransaction(transactionHash);
        }
    }
}
