using System;
using System.Collections.Generic;
using NirvanaCommon;
using Preloader;
using Version1;

namespace PreloadVersion1
{
    static class Program
    {
        static void Main()
        {
            (List<int> positions, _) = Preloader.Preloader.GetPositions(Datasets.PedigreeTsvPath);

            var benchmark    = new Benchmark();
            int numPreloaded = V1Preloader.Preload(GRCh37.Chr1, positions, "0.05");

            Console.WriteLine();
            Console.WriteLine($"- {numPreloaded:N0} variants preloaded.");
            Console.WriteLine($"- elapsed time: {benchmark.GetElapsedTime()}");
            MemoryUtilities.PrintAllocations();
        }
    }
}