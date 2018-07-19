using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace PoaScannerTest
{
    internal static class Program
    {
        private const string TestNetNodeUrl = "https://sokol-trace.poa.network/";

        private const string MainNetNodeUrl = "https://core.poa.network/";

        private const int DefaultBlockCount = 10;

        private static async Task Main(string[] args)
        {
            var useMainnet = args.Contains("--mainnet", StringComparer.OrdinalIgnoreCase);
            var blocksToScan = DefaultBlockCount;
            var countIndex = Array.IndexOf(args, "--count");
            if (countIndex != -1 && countIndex != args.Length - 1)
            {
                if (int.TryParse(args[countIndex + 1], out var count) && count > 0)
                    blocksToScan = count;
            }

            var web3 = new Web3(useMainnet ? MainNetNodeUrl : TestNetNodeUrl)
            {
                Client = {OverridingRequestInterceptor = new LoggingRequestInterceptor()}
            };

            var blockCountHex = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            var blockCount = (long) blockCountHex.Value;
            Console.WriteLine($"Block count = {blockCount}");

            var startBlock = blockCount - blocksToScan;
            Console.WriteLine($"Scanning block range {startBlock}...{blockCount}");

            var sw = Stopwatch.StartNew();
            for (var i = startBlock; i < blockCount; ++i)
            {
                var arg = new HexBigInteger(new BigInteger(i));
                var block = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(arg);
                BlockCallback(block);
            }

            Console.WriteLine($"{blocksToScan} blocks scanned in {sw.ElapsedMilliseconds} ms");
        }

        private static void BlockCallback(BlockWithTransactions block)
        {
            Console.WriteLine($"Transaction sum: {block.Transactions.Select(t => (long)t.Value.Value).Sum() * 1.0 / 1_000_000_000_000_000_000}");
        }
    }
}
