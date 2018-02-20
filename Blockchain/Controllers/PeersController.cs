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
    [Route("api/peers")]
    [Produces("application/json")]
    public class PeersController : BlockchainController
    {
        public PeersController(IBlockchainService blockchainService) : base(blockchainService) { }

        /// <summary>
        /// Get registered node peers
        /// </summary>
        [HttpGet]
        public List<string> Get()
        {
            return _blockchainService.GetPeers();
        }

        /// <summary>
        /// Add new node peer
        /// </summary>
        [HttpPost]
        public void Post([FromBody]AddPeerRequest data)
        {
            _blockchainService.AddPeer(data.PeerUrl);
        }
    }
}