using Blockchain.Models;
using BlockchainCore;
using BlockchainCore.Utils;
using RestSharp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Miner
{
    class Program
    {
        static CancellationTokenSource runningTaskSource;
        static string myAddress = "123";
        static string url = "http://localhost:5000"; //TODO: un-hard-code this shit

        static void Main(string[] args)
        {
            DoStuff().Wait();
        }

        static async Task DoStuff()
        {
            while (true)
            {
                var bi = GetLatestBlockInfo(myAddress);
                var t = MineAsync(bi.BlockDataHash, bi.Difficulty);

                //if same block do not cancel job.
                await Task.Delay(5000);

                if (t.IsCompleted)
                {
                    SubmitMinedBlockInfo(myAddress, t.Result.Item1, t.Result.Item2);
                }
            }
        }

        static void SubmitMinedBlockInfo(string myAddress, ulong nonce, DateTime dateCreated)
        {
            var p = new Parameter() { Name = "address", Value = myAddress, Type = ParameterType.UrlSegment };

            var body = new MinedBlockInfoRequest
            {
                DateCreated = dateCreated,
                Nonce = nonce
            };

            HttpUtils.DoApiPost<MinedBlockInfoRequest, SubmitBlockResponse>(url, "api/mining/submit-block/{address}", body, p);
        }

        static MiningBlockInfoResponse GetLatestBlockInfo(string address)
        {
            var p = new Parameter() { Name = "address", Value = address, Type = ParameterType.UrlSegment };
            return HttpUtils.DoApiGet<MiningBlockInfoResponse>(url, "api/mining/get-block/{address}", p);
        }

        static async Task<Tuple<ulong, DateTime>> MineAsync(string blockDataHash, int difficulty)
        {
            if (null != runningTaskSource && !runningTaskSource.IsCancellationRequested)
            {
                runningTaskSource.Cancel();
            }
            
            runningTaskSource = new CancellationTokenSource();
            var result = await Task.Run<Tuple<ulong, DateTime>>(() => Mine(blockDataHash, difficulty), runningTaskSource.Token);

            runningTaskSource = null;

            return result;
        }

        static Tuple<ulong, DateTime> Mine(string blockDataHash, int difficulty)
        {
            var dt = DateTime.UtcNow;
            var str = blockDataHash;
            str += dt.ToString("o");
            var prefix = "".PadLeft(difficulty, '0');

            var nonce = (ulong)0;
            while (true)
            {
                var hash = CryptoUtils.GetSha256Hex(str + nonce.ToString());
                if (hash.StartsWith(prefix))
                    return new Tuple<ulong, DateTime>(nonce, dt);

                nonce++;
            }
        }
    }
}
