using System;
using NirvanaCommon;
using Preloader;

namespace PreloadBaseline
{
    internal static class Program
    {
        private static void Main()
        {
            VcfPreloadData preloadData = Preloader.Preloader.GetPositions(Preloader.Preloader.GetLines(Datasets.PedigreeTsvPath));
            int numPreloaded = Baseline.Preload(GRCh37.Chr1, preloadData.Positions);
            Console.WriteLine($"- {numPreloaded:N0} variants preloaded.");
        }
    }
}