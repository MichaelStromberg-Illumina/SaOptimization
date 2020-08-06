using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Benchmarks.Data;
using NirvanaCommon;
using PreloadBaseline;
using Version1;
using Version2;
using Version3;
using Version4;
using Version5;

namespace Benchmarks
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    [MemoryDiagnoser]
    public class Preloading
    {
        private const int ExpectedPreloadedVariants     = 304_636;
        private const int ExpectedPreloadedVariantsOrig = 304_403;
        private const int ExpectedPreloadedAlleles      = 180_445;

        private readonly VcfPreloadData _preloadData = PreloadedData.PedigreeData;

        [Benchmark(Baseline = true)]
        public int Current()
        {
            int numPreloadedVariants = Baseline.Preload(GRCh37.Chr1, _preloadData.Positions);
            if (numPreloadedVariants != ExpectedPreloadedVariantsOrig) throw new InvalidDataException();
            return numPreloadedVariants;
        }

        [Benchmark]
        public int RareBitVector_5pct()
        {
            const string threshold            = "0.05";
            int          numPreloadedVariants = V1Preloader.Preload(GRCh37.Chr1, _preloadData.Positions, threshold);
            if (numPreloadedVariants != ExpectedPreloadedVariants) throw new InvalidDataException();
            return numPreloadedVariants;
        }

        [Benchmark]
        public int TwoBitVectors_5pct()
        {
            int numPreloadedVariants = V2Preloader.Preload(GRCh37.Chr1, _preloadData.Positions);
            if (numPreloadedVariants != ExpectedPreloadedVariants) throw new InvalidDataException();
            return numPreloadedVariants;
        }

        [Benchmark]
        public int NoBitVector_5pct()
        {
            int numPreloadedVariants = V3Preloader.Preload(GRCh37.Chr1, _preloadData.Positions);
            if (numPreloadedVariants != ExpectedPreloadedVariants) throw new InvalidDataException();
            return numPreloadedVariants;
        }

        [Benchmark]
        public int RareBitVector_5pct_Opt()
        {
            int numPreloadedVariants =
                V4Preloader.Preload(GRCh37.Chr1, _preloadData.Positions, _preloadData.PositionAlleleHashTable);
            if (numPreloadedVariants != ExpectedPreloadedAlleles) throw new InvalidDataException();
            return numPreloadedVariants;
        }

        [Benchmark]
        public int XorFilter_5pct()
        {
            int numPreloadedVariants = V5Preloader.Preload(GRCh37.Chr1, _preloadData.PositionAlleles,
                _preloadData.PositionAlleleHashTable);
            if (numPreloadedVariants != ExpectedPreloadedAlleles) throw new InvalidDataException();
            return numPreloadedVariants;
        }
    }
}