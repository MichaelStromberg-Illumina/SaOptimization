using System;
using System.Collections.Generic;
using NirvanaCommon;
using Preloader;
using Version2;

namespace PreloadVersion2
{
    internal static class Program
    {
        private static void Main()
        {
            (List<int> positions, _) = Preloader.Preloader.GetPositions(Datasets.PedigreeTsvPath);
            
            var benchmark = new Benchmark();
            int numPreloaded = V2Preloader.Preload(GRCh37.Chr1, positions);
            Console.WriteLine();
            Console.WriteLine($"- {numPreloaded:N0} variants preloaded.");
            Console.WriteLine($"- elapsed time: {benchmark.GetElapsedTime()}");
            MemoryUtilities.PrintAllocations();
        }
    }
}