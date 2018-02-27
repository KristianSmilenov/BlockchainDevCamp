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
        static string url = "http://localhost:5000";

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: \t dotnet Miner.dll <NODE_URL> <Miner_Address>");
                return;
            }

            url = args[0];
            myAddress = args[1];

            DoStuff().Wait();
        }

        static async Task DoStuff()
        {
            MiningBlockInfoResponse lastBlockInfo = null;
            Task<Tuple<ulong, DateTime>> t = null;

            while (true)
            {
                var bi = GetLatestBlockInfo(myAddress);
                if (bi == null) {
                    Console.WriteLine("No connection to server, sleeping for a second..");
                    await Task.Delay(1000);
                    continue;
                }

                if (t == null)//nothing here, let's do some mining
                {
                    Console.WriteLine($"[{DateTime.Now}] Mining started, block {bi.Index}, hash: {bi.BlockDataHash}");
                    t = MineAsync(bi.BlockDataHash, bi.Difficulty);
                }
                else if (t.IsCompleted) //we're done, let's get some $$
                {
                    Console.WriteLine($"F[{DateTime.Now}] ound a result for block {bi.Index}, awaiting payment :)");
                    SubmitMinedBlockInfo(myAddress, t.Result.Item1, t.Result.Item2);
                    t = null;
                }
                else if (lastBlockInfo.BlockDataHash != bi.BlockDataHash)//new block, let's cancel and start mining it
                {
                    Console.WriteLine($"[{DateTime.Now}] New block suggested, ditching the old one and starting the new one");
                    t = null;
                }
                else //same block, and we're still mining, so will sit for a second and see what comes next;
                {
                    await Task.Delay(1000);
                }

                lastBlockInfo = bi;
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
            str += DateTimeUtils.GetISO8601DateFormat(dt);
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
