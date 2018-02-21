using BlockchainCore.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Miner
{
    class Program
    {
        static CancellationTokenSource runningTaskSource;

        static void Main(string[] args)
        {
            //check for new block - if yes, cancel mining and start mining new block
        }

        static async Task MineAsync(string blockDataHash, int difficulty)
        {
            if (null != runningTaskSource && !runningTaskSource.IsCancellationRequested)
            {
                runningTaskSource.Cancel();
            }

            var blockData = "";
            var diff = 1;
            runningTaskSource = new CancellationTokenSource();
            var task = Task.Run(() => Mine(blockData, diff), runningTaskSource.Token);

            await task;
            runningTaskSource = null;
        }

        static string Mine(string blockDataHash, int difficulty)
        {
            var dateCreated = ((UInt32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
            var str = blockDataHash;
            str += dateCreated;
            var prefix = "".PadLeft(difficulty, '0');

            var nonce = 0L;
            while (true)
            {
                var hash = CryptoUtils.GetSha256String(str + nonce.ToString());
                if (hash.StartsWith(prefix))
                    return hash;

                nonce++;
            }
        }
    }
}
