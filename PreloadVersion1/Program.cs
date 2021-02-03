using System;
using NirvanaCommon;
using Preloader;
using Version1;
using Version1.Utilities;

namespace PreloadVersion1
{
    internal static class Program
    {
        private static void Main()
        {
            VcfPreloadData preloadData =
                Preloader.Preloader.GetPositions(Preloader.Preloader.GetLines(Datasets.PedigreeTsvPath));
            (string saPath, string indexPath) = SaPath.GetPaths(SupplementaryAnnotation.Directory, "0.05");

            var benchmark    = new Benchmark();
            int numPreloaded = V1Preloader.Preload(GRCh37.Chr1, saPath, indexPath, preloadData.Positions);

            Console.WriteLine();
            Console.WriteLine($"- {numPreloaded:N0} variants preloaded.");
            Console.WriteLine($"- elapsed time: {benchmark.GetElapsedTime()}");
            MemoryUtilities.PrintAllocations();
        }
    }
}