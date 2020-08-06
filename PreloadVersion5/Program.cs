using System;
using NirvanaCommon;
using Preloader;
using Version5;

namespace PreloadVersion5
{
    internal static class Program
    {
        private static void Main()
        {
            VcfPreloadData preloadData = Preloader.Preloader.GetPositions(Preloader.Preloader.GetLines(Datasets.PedigreeTsvPath));

            var benchmark    = new Benchmark();
            int numPreloaded = V5Preloader.Preload(GRCh37.Chr1, preloadData.PositionAlleles, preloadData.PositionAlleleHashTable);

            Console.WriteLine();
            Console.WriteLine($"- {numPreloaded:N0} variants preloaded.");
            Console.WriteLine($"- elapsed time: {benchmark.GetElapsedTime()}");
            MemoryUtilities.PrintAllocations();
        }
    }
}