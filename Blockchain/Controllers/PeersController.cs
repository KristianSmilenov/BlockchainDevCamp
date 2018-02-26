using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blockchain.Models;
using Blockchain.Services;
using BlockchainCore.Models;
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
        /// Get node peers
        /// </summary>
        [HttpGet]
        public List<Peer> Get()
        {
            return _blockchainService.GetPeers();
        }

        /// <summary>
        /// Add new node peer
        /// </summary>
        [HttpPost]
        public void Post([FromBody]Peer data)
        {
            _blockchainService.AddPeer(data);
        }

        /// <summary>
        /// Get peers network
        /// </summary>
        [HttpGet("network")]
        public PeersNetwork GetNetwork()
        {
            return _blockchainService.GetPeersNetwork();
        }
    }
}