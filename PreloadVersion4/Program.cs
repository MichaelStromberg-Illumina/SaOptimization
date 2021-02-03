using System;
using NirvanaCommon;
using Preloader;
using Version4;
using Version4.Utilities;

namespace PreloadVersion4
{
    internal static class Program
    {
        private static void Main()
        {
            VcfPreloadData preloadData = Preloader.Preloader.GetPositions(Preloader.Preloader.GetLines(Datasets.PedigreeTsvPath));
            (string saPath, string indexPath) = SaPath.GetPaths(SupplementaryAnnotation.Directory);

            var benchmark = new Benchmark();
            int numPreloaded =
                V4Preloader.Preload(GRCh37.Chr1, saPath, indexPath, preloadData.Positions, preloadData.PositionAlleleHashTable);

            Console.WriteLine();
            Console.WriteLine($"- {numPreloaded:N0} variants preloaded.");
            Console.WriteLine($"- elapsed time: {benchmark.GetElapsedTime()}");
            MemoryUtilities.PrintAllocations();
        }
    }
}