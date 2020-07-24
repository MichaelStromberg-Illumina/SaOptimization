using System;
using System.Collections.Generic;
using NirvanaCommon;
using Preloader;

namespace PreloadBaseline
{
    static class Program
    {
        static void Main()
        {
            List<int> positions = Preloader.Preloader.GetPositions(Datasets.PedigreePreloadPath);

            int numPreloaded = Baseline.Preload(GRCh37.Chr1, positions);
            Console.WriteLine($"- {numPreloaded:N0} variants preloaded.");
        }
    }
}