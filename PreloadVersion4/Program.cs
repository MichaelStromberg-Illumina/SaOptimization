using System;
using System.Collections.Generic;
using NirvanaCommon;
using Preloader;
using Version4;

namespace PreloadVersion4
{
    static class Program
    {
        static void Main()
        {
            List<int> positions = Preloader.Preloader.GetPositions(Datasets.PedigreePreloadPath);
            
            var benchmark = new Benchmark();
            int numPreloaded = V4Preloader.Preload(GRCh37.Chr1, positions);
            Console.WriteLine();
            Console.WriteLine($"- {numPreloaded:N0} variants preloaded.");
            Console.WriteLine($"- elapsed time: {benchmark.GetElapsedTime()}");
            MemoryUtilities.PrintAllocations();
        }
    }
}