using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blockchain.Models;
using BlockchainCore;
using BlockchainCore.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebFaucet
{
    [Route("api/[controller]")]
    public class TransactionsController : Controller
    {
        private AppSettings appSettings;

        public TransactionsController(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        // POST api/<controller>
        [HttpPost]
        public TransactionHashInfo Post([FromBody]TransactionModel request)
        {
            var privateKey = CryptoUtils.HexToByteArray(appSettings.PrivateKey);
            var publicKey = CryptoUtils.GetPublicFor(privateKey);
            var date = DateTime.Now;

            var dataToSign = new TransactionData
            {
                From = appSettings.FaucetAddress,
                To = request.RecipientAddress,
                Value = request.Value,
                Fee = 0,
                SenderPubKey = CryptoUtils.ByteArrayToHex(publicKey),
                DateCreated = date
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(dataToSign);
            var msgHash = CryptoUtils.GetSha256Bytes(json);
            var signedMessage = CryptoUtils.BouncyCastleSign(msgHash, privateKey);

            //send http request to node;
            var d = new TransactionDataSigned
            {
                From = appSettings.FaucetAddress,
                To = request.RecipientAddress,
                Value = request.Value,
                Fee = 0,
                SenderPubKey = CryptoUtils.ByteArrayToHex(publicKey),
                DateCreated = date,
                SenderSignature = CryptoUtils.ByteArrayToHex(signedMessage)
            };

            var res = HttpUtils.DoApiPost<TransactionDataSigned, TransactionHashInfo>(request.NodeUrl, "api/transactions", d);

            return res;
        }
    }
}
