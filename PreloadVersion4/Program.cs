using System;
using System.Collections.Generic;
using NirvanaCommon;
using Preloader;
using Version4;

namespace PreloadVersion4
{
    internal static class Program
    {
        private static void Main()
        {
            (List<int> positions, LongHashTable positionAlleles) =
                Preloader.Preloader.GetPositions(Datasets.PedigreeTsvPath);

            var benchmark    = new Benchmark();
            int numPreloaded = V4Preloader.Preload(GRCh37.Chr1, positions, positionAlleles);

            Console.WriteLine();
            Console.WriteLine($"- {numPreloaded:N0} variants preloaded.");
            Console.WriteLine($"- elapsed time: {benchmark.GetElapsedTime()}");
            MemoryUtilities.PrintAllocations();
        }
    }
}