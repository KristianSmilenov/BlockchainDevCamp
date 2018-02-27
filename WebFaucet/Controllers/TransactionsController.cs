using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blockchain.Models;
using BlockchainCore;
using BlockchainCore.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebFaucet.Models;
using WebFaucet.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebFaucet
{
    [Route("api/[controller]")]
    public class TransactionsController : Controller
    {
        protected readonly IDBService _dbService;
        
        private AppSettings _appSettings;

        public TransactionsController(IOptions<AppSettings> appSettings, IDBService dbService)
        {
            _appSettings = appSettings.Value;
            _dbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
        }

        [HttpPost]
        public TransactionHashInfo Post([FromBody]TransactionModel request)
        {
            if (!ShouldSendCoins(request.RecipientAddress))
            {
                return new TransactionHashInfo()
                {
                    IsValid = false,
                    ErrorMessage = "User has received coins in the last hour."
                };
            }

            if (request.Value > _appSettings.MaxAddressDonationPerHour)
            {
                return new TransactionHashInfo()
                {
                    IsValid = false,
                    ErrorMessage = String.Format("User cannot receive more than {0} coins per hour.", _appSettings.MaxAddressDonationPerHour)
                };
            }

            var privateKey = CryptoUtils.HexToByteArray(_appSettings.PrivateKey);
            var publicKey = CryptoUtils.GetPublicFor(privateKey);
            var date = DateTime.UtcNow;
            TransactionData dataToSign = GetDataToSign(request, publicKey, date);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(dataToSign);
            var msgHash = CryptoUtils.GetSha256Bytes(json);
            var signedMessage = CryptoUtils.BouncyCastleSign(msgHash, privateKey);

            // send http request to node
            var signedTransactionData = new TransactionDataSigned
            {
                From = _appSettings.FaucetAddress,
                To = request.RecipientAddress,
                Value = request.Value,
                Fee = 0,
                SenderPubKey = CryptoUtils.ByteArrayToHex(publicKey),
                DateCreated = date,
                SenderSignature = CryptoUtils.ByteArrayToHex(signedMessage)
            };

            var res = HttpUtils.DoApiPost<TransactionDataSigned, TransactionHashInfo>(request.NodeUrl, "api/transactions", signedTransactionData);
            if (res.IsValid)
            {
                // add transaction record with limits
                var recordHistory = new TransactionHistory
                {
                    Address = request.RecipientAddress,
                    DateReceived = res.DateReceived,
                    Amount = request.Value
                };
                _dbService.Add(request.RecipientAddress, recordHistory);
            }
            return res;
        }

        private TransactionData GetDataToSign(TransactionModel request, byte[] publicKey, DateTime date)
        {
            return new TransactionData
            {
                From = _appSettings.FaucetAddress,
                To = request.RecipientAddress,
                Value = request.Value,
                Fee = 0,
                SenderPubKey = CryptoUtils.ByteArrayToHex(publicKey),
                DateCreated = date
            };
        }

        private bool ShouldSendCoins(string address)
        {
            TransactionHistory addressHistory;
            var hasSentCoins = _dbService.TryGetValue(address, out addressHistory);
            if (hasSentCoins)
            {
                if (addressHistory.DateReceived.AddHours(1) < DateTime.UtcNow)
                {
                    return true;
                }
                return false;
            }
            return true;
        }
    }
}
