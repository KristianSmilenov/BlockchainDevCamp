using Blockchain.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Controllers
{
    [Route("api/transactions")]
    [Produces("application/json")]
    public class TransactionsController : Controller
    {
        [HttpPost]
        public void Post([FromBody]TransactionRequest data)
        {

        }
    }
}
