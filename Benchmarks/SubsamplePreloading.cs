using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Benchmarks.Data;
using NirvanaCommon;
using PreloadBaseline;
using Preloader;
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
    public class SubsamplePreloading
    {
        private readonly Dictionary<int, VcfPreloadData> _preloadDataDict;

        public int[] NumSamplesValues => new[]
        {
            1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 150000, 200000, 250000
        };

        [ParamsSource(nameof(NumSamplesValues))]
        // ReSharper disable once UnassignedField.Global
        public int NumSamples;

        public SubsamplePreloading()
        {
            var shuffledLines = new List<string>(PreloadedData.PedigreeLines);
            shuffledLines.Shuffle();
            
            _preloadDataDict = new Dictionary<int, VcfPreloadData>();
            
            foreach (int numSamples in NumSamplesValues)
            {
                _preloadDataDict[numSamples] = Subsampler.Subsample(shuffledLines, numSamples);
            }
        }

        [Benchmark(Baseline = true)]
        public int Current() => Baseline.Preload(GRCh37.Chr1, _preloadDataDict[NumSamples].Positions);

        [Benchmark]
        public int RareBitVector_5pct() => V1Preloader.Preload(GRCh37.Chr1, _preloadDataDict[NumSamples].Positions, "0.05");

        [Benchmark]
        public int TwoBitVectors_5pct() => V2Preloader.Preload(GRCh37.Chr1, _preloadDataDict[NumSamples].Positions);

        [Benchmark]
        public int NoBitVector_5pct() => V3Preloader.Preload(GRCh37.Chr1, _preloadDataDict[NumSamples].Positions);

        [Benchmark]
        public int RareBitVector_5pct_Opt()
        {
            VcfPreloadData preloadData = _preloadDataDict[NumSamples];
            return V4Preloader.Preload(GRCh37.Chr1, preloadData.Positions, preloadData.PositionAlleleHashTable);
        }
        
        [Benchmark]
        public int XorFilter_5pct()
        {
            VcfPreloadData preloadData = _preloadDataDict[NumSamples];
            return V5Preloader.Preload(GRCh37.Chr1, preloadData.PositionAlleles, preloadData.PositionAlleleHashTable);
        }
    }
}