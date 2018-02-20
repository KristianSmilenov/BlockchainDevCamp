using Blockchain.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Controllers
{
    public class BlockchainController : Controller
    {
        protected readonly IBlockchainService _blockchainService;

        public BlockchainController(IBlockchainService blockchainService)
        {
            _blockchainService = blockchainService ?? throw new ArgumentNullException(nameof(blockchainService));
        }
    }
}
