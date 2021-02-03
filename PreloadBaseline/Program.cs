using System;
using NirvanaCommon;
using Preloader;

namespace PreloadBaseline
{
    internal static class Program
    {
        private static void Main()
        {
            (string saPath, string indexPath) = SaPath.GetPaths(SupplementaryAnnotation.Directory);

            var benchmark = new Benchmark();
            VcfPreloadData preloadData =
                Preloader.Preloader.GetPositions(Preloader.Preloader.GetLines(Datasets.PedigreeTsvPath));
            int numPreloaded = Baseline.Preload(GRCh37.Chr1, saPath, indexPath, preloadData.Positions);

            Console.WriteLine();
            Console.WriteLine($"- {numPreloaded:N0} variants preloaded.");
            Console.WriteLine($"- elapsed time: {benchmark.GetElapsedTime()}");
            MemoryUtilities.PrintAllocations();
        }
    }
}