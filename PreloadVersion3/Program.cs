using System;
using System.Collections.Generic;
using NirvanaCommon;
using Preloader;
using Version3;

namespace PreloadVersion3
{
    static class Program
    {
        static void Main()
        {
            (List<int> positions, _) = Preloader.Preloader.GetPositions(Datasets.PedigreeTsvPath);
            
            var benchmark = new Benchmark();
            int numPreloaded = V3Preloader.Preload(GRCh37.Chr1, positions);
            Console.WriteLine();
            Console.WriteLine($"- {numPreloaded:N0} variants preloaded.");
            Console.WriteLine($"- elapsed time: {benchmark.GetElapsedTime()}");
            MemoryUtilities.PrintAllocations();
        }
    }
}