using System;
using NirvanaCommon;
using Preloader;
using Version4;

namespace PreloadVersion4
{
    internal static class Program
    {
        private static void Main()
        {
            VcfPreloadData preloadData = Preloader.Preloader.GetPositions(Preloader.Preloader.GetLines(Datasets.PedigreeTsvPath));

            var benchmark = new Benchmark();
            int numPreloaded =
                V4Preloader.Preload(GRCh37.Chr1, preloadData.Positions, preloadData.PositionAlleleHashTable);

            Console.WriteLine();
            Console.WriteLine($"- {numPreloaded:N0} variants preloaded.");
            Console.WriteLine($"- elapsed time: {benchmark.GetElapsedTime()}");
            MemoryUtilities.PrintAllocations();
        }
    }
}