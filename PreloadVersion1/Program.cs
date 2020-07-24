using System;
using System.Collections.Generic;
using NirvanaCommon;
using VariantGrouping;
using Version1;

namespace PreloadVersion1
{
    static class Program
    {
        static void Main()
        {
            List<int> positions = Preloader.Preloader.GetPositions(@"E:\Data\Nirvana\gnomAD_chr1_pedigree_position_new.txt");
            
            // var variants = new List<PreloadVariant>(positions.Count);
            // foreach (int position in positions) variants.Add(new PreloadVariant(position, null, VariantType.SNV));
            
            var benchmark = new Benchmark();
            int numPreloaded = V1Preloader.Preload(GRCh37.Chr1, positions);
            Console.WriteLine();
            Console.WriteLine($"- {numPreloaded:N0} variants preloaded.");
            Console.WriteLine($"- elapsed time: {benchmark.GetElapsedTime()}");
            MemoryUtilities.PrintAllocations();
        }
    }
}