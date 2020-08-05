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
        private readonly List<int>     _positions;
        private readonly LongHashTable _positionAlleles;

        private const int ExpectedPreloadedVariants = 304_636;
        private const int ExpectedPreloadedAlleles  = 262_528;

        public Preloading() =>
            (_positions, _positionAlleles) = Preloader.Preloader.GetPositions(Datasets.PedigreeTsvPath);

        [Benchmark(Baseline = true)]
        public int Current()
        {
            int numPreloadedVariants = Baseline.Preload(GRCh37.Chr1, _positions);
            if (numPreloadedVariants != ExpectedPreloadedVariants) throw new InvalidDataException();
            return numPreloadedVariants;
        }

        [Benchmark(Baseline = true)]
        public int RareBitVector_5pct()
        {
            const string threshold = "0.05";
            int    numPreloadedVariants = V1Preloader.Preload(GRCh37.Chr1, _positions, threshold);
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
        public int RareBitVector_5pct_Opt()
        {
            int numPreloadedVariants = V4Preloader.Preload(GRCh37.Chr1, _positions, _positionAlleles);
            if (numPreloadedVariants != ExpectedPreloadedAlleles) throw new InvalidDataException();
            return numPreloadedVariants;
        }
    }
}