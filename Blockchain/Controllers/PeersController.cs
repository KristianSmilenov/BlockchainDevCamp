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

        [HttpGet]
        public List<string> Get()
        {
            return _blockchainService.GetPeers();
        }

        [HttpPost]
        public void Post([FromBody]AddPeerRequest data)
        {
            _blockchainService.AddPeer(data.PeerUrl);
        }
    }
}