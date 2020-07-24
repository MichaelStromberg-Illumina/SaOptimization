using System;
using System.Collections.Generic;
using NirvanaCommon;

namespace PreloadBaseline
{
    class Program
    {
        static void Main()
        {
            List<int> positions = Preloader.Preloader.GetPositions(@"E:\Data\Nirvana\gnomAD_chr1_pedigree_position_new.txt");

            int numPreloaded = Baseline.Preload(GRCh37.Chr1, positions);
            Console.WriteLine($"- {numPreloaded:N0} variants preloaded.");
        }
    }
}