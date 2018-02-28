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
                    Console.WriteLine($"[{DateTime.UtcNow}] Mining started, block {bi.Index}, hash: {bi.BlockDataHash}, difficulty: {bi.Difficulty}");
                    t = MineAsync(bi.BlockDataHash, bi.Difficulty);
                    lastBlockInfo = bi;
                }
                else if (t.IsCompleted) //we're done, let's get some $$
                {
                    if (SubmitMinedBlockInfo(myAddress, t.Result.Item1, t.Result.Item2, lastBlockInfo.BlockDataHash))
                    {
                        Console.WriteLine($"[{DateTime.UtcNow}] Found a result for block {lastBlockInfo.Index}, $$ is on the way");
                    }

                    t = null;
                }
                else if (lastBlockInfo.Index < bi.Index)
                {//new block, start mining it
                    Console.WriteLine($"[{DateTime.UtcNow}] New block, ditching the old one and starting the new one");
                    t = null;
                }
                else if (lastBlockInfo.BlockDataHash != bi.BlockDataHash && lastBlockInfo.Transactions[0].Value < bi.Transactions[0].Value)
                {//better reward, let's cancel and start mining again
                    Console.WriteLine($"[{DateTime.UtcNow}] Higher reward, ditching the old job and starting the new one");
                    t = null;
                }
                else //same block, and we're still mining, so will sit for a second and see what comes next;
                {
                    await Task.Delay(1000);
                }
            }
        }

        static bool SubmitMinedBlockInfo(string myAddress, ulong nonce, DateTime dateCreated, string blockHash)
        {
            var body = new MinedBlockInfoRequest
            {
                DateCreated = dateCreated,
                Nonce = nonce,
                BlockDataHash = blockHash
            };

            var res = HttpUtils.DoApiPost<MinedBlockInfoRequest, SubmitBlockResponse>(url, "api/mining/submit-block/", body);
            if (res == null)
            {
                Console.WriteLine($"[{DateTime.UtcNow}] Error occured while trying to call the node API..");
                return false;
            }

            if (res.Status == BlockResponseStatus.Error)
            {
                Console.WriteLine($"Error {res.Message}");
                return false;
            }

            return true;
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
