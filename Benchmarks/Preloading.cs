using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using NirvanaCommon;
using PreloadBaseline;
using Preloader;
using Version1;
using Version2;
using Version3;
using Version4;

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
        [Arguments("0.01")]
        [Arguments("0.02")]
        [Arguments("0.03")]
        [Arguments("0.04")]
        [Arguments("0.05")]
        [Arguments("0.06")]
        [Arguments("0.07")]
        [Arguments("0.09")]
        [Arguments("0.14")]
        [Arguments("0.19")]
        [Arguments("0.24")]
        [Arguments("0.29")]
        [Arguments("0.34")]
        [Arguments("0.39")]
        [Arguments("0.44")]
        [Arguments("0.49")]
        [Arguments("0.54")]
        [Arguments("0.59")]
        [Arguments("0.64")]
        [Arguments("0.69")]
        [Arguments("0.74")]
        [Arguments("0.79")]
        [Arguments("0.84")]
        [Arguments("0.89")]
        [Arguments("0.94")]
        [Arguments("0.99")]
        public int RareBitVector(string threshold)
        {
            int numPreloadedVariants = V1Preloader.Preload(GRCh37.Chr1, _positions, threshold);
            if (numPreloadedVariants != ExpectedPreloadedVariants) throw new InvalidDataException();
            return numPreloadedVariants;
        }
        
        [Benchmark]
        public int TwoBitVectors_5pct()
        {
            int numPreloadedVariants = V2Preloader.Preload(GRCh37.Chr1, _positions);
            if (numPreloadedVariants != ExpectedPreloadedVariants) throw new InvalidDataException();
            return numPreloadedVariants;
        }
        
        [Benchmark]
        public int NoBitVector_5pct()
        {
            int numPreloadedVariants = V3Preloader.Preload(GRCh37.Chr1, _positions);
            if (numPreloadedVariants != ExpectedPreloadedVariants) throw new InvalidDataException();
            return numPreloadedVariants;
        }
        
        [Benchmark]
        public int NRareBitVector_5pct_Opt()
        {
            int numPreloadedVariants = V4Preloader.Preload(GRCh37.Chr1, _positions);
            if (numPreloadedVariants != ExpectedPreloadedVariants) throw new InvalidDataException();
            return numPreloadedVariants;
        }
    }
}