using System;
using System.Collections.Generic;
using NirvanaCommon;
using Version2;

namespace PreloadVersion2
{
    static class Program
    {
        static void Main()
        {
            List<int> positions = Preloader.Preloader.GetPositions(@"E:\Data\Nirvana\gnomAD_chr1_pedigree_position_new.txt");
            
            var benchmark = new Benchmark();
            int numPreloaded = V2Preloader.Preload(GRCh37.Chr1, positions);
            Console.WriteLine();
            Console.WriteLine($"- {numPreloaded:N0} variants preloaded.");
            Console.WriteLine($"- elapsed time: {benchmark.GetElapsedTime()}");
            MemoryUtilities.PrintAllocations();
        }
    }
}