using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using NirvanaCommon;
using PreloadBaseline;
using Preloader;
using Version1;
using Version2;

namespace Benchmarks
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    [MemoryDiagnoser]
    public class Preloading
    {
        private readonly List<int> _positions;

        private const int ExpectedPreloadedVariants = 304_636;

        public Preloading() => _positions = Preloader.Preloader.GetPositions(Datasets.PedigreePreloadPath);

        [Benchmark(Baseline = true)]
        public int Current()
        {
            int numPreloadedVariants = Baseline.Preload(GRCh37.Chr1, _positions);
            if (numPreloadedVariants != ExpectedPreloadedVariants) throw new InvalidDataException();
            return numPreloadedVariants;
        }

        [Benchmark]
        public int V1()
        {
            int numPreloadedVariants = V1Preloader.Preload(GRCh37.Chr1, _positions);
            if (numPreloadedVariants != ExpectedPreloadedVariants) throw new InvalidDataException();
            return numPreloadedVariants;
        }

        [Benchmark]
        public int V2()
        {
            int numPreloadedVariants = V2Preloader.Preload(GRCh37.Chr1, _positions);
            if (numPreloadedVariants != ExpectedPreloadedVariants) throw new InvalidDataException();
            return numPreloadedVariants;
        }
    }
}