using System;
using System.IO;
using NirvanaCommon;
using Preloader;
using Version5;
using Version5.Utilities;

namespace PreloadVersion5
{
    internal static class Program
    {
        private static void Main(string [] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine($"USAGE: {Path.GetFileName(Environment.GetCommandLineArgs()[0])} <SA directory> <common threshold> <common block size> <rare block size>");
                Environment.Exit(1);
            }
            
            string saDir           = args[0];
            string commonThreshold = args[1];
            int    commonBlockSize = int.Parse(args[2]);
            int    rareBlockSize   = int.Parse(args[3]);
            
            VcfPreloadData preloadData =
                Preloader.Preloader.GetPositions(Preloader.Preloader.GetLines(Datasets.PedigreeTsvPath));
            (string saPath, string indexPath) = SaPath.GetPaths(saDir, commonThreshold, commonBlockSize, rareBlockSize);

            var benchmark = new Benchmark();
            int numPreloaded = V5Preloader.Preload(GRCh37.Chr1, saPath, indexPath, preloadData.PositionAlleles,
                preloadData.PositionAlleleHashTable);

            Console.WriteLine();
            Console.WriteLine($"- {numPreloaded:N0} variants preloaded.");
            Console.WriteLine($"- elapsed time: {benchmark.GetElapsedTime()}");
            MemoryUtilities.PrintAllocations();
        }
    }
}