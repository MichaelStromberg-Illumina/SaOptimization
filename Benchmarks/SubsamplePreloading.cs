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
    [RankColumn, MinColumn, MaxColumn]
    [MemoryDiagnoser]
    [MaxRelativeError(0.005)]
    public class SubsamplePreloading
    {
        private readonly Dictionary<int, VcfPreloadData> _preloadDataDict;
        private readonly string _saDir = SupplementaryAnnotation.Directory;
        
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
        public int Current()
        {
            (string saPath, string indexPath) = SaPath.GetPaths(SupplementaryAnnotation.DevelopDirectory);
            FileCacheBlaster.Blast(saPath, indexPath);
            return Baseline.Preload(GRCh37.Chr1, saPath, indexPath, _preloadDataDict[NumSamples].Positions);
        }

        [Benchmark]
        public int RareBitVector_5pct()
        {
            (string saPath, string indexPath) = Version1.Utilities.SaPath.GetPaths(_saDir, "0.05");
            FileCacheBlaster.Blast(saPath, indexPath);
            return V1Preloader.Preload(GRCh37.Chr1, saPath, indexPath, _preloadDataDict[NumSamples].Positions);
        }

        [Benchmark]
        public int TwoBitVectors_5pct()
        {
            (string saPath, string indexPath) = Version2.Utilities.SaPath.GetPaths(_saDir);
            FileCacheBlaster.Blast(saPath, indexPath);
            return V2Preloader.Preload(GRCh37.Chr1, saPath, indexPath, _preloadDataDict[NumSamples].Positions);
        }

        [Benchmark]
        public int NoBitVector_5pct()
        {
            (string saPath, string indexPath) = Version3.Utilities.SaPath.GetPaths(_saDir);
            FileCacheBlaster.Blast(saPath, indexPath);
            return V3Preloader.Preload(GRCh37.Chr1, saPath, indexPath, _preloadDataDict[NumSamples].Positions);
        }

        [Benchmark]
        public int RareBitVector_Span_AI_5pct()
        {
            (string saPath, string indexPath) = Version4.Utilities.SaPath.GetPaths(_saDir);
            FileCacheBlaster.Blast(saPath, indexPath);

            VcfPreloadData preloadData = _preloadDataDict[NumSamples];
            return V4Preloader.Preload(GRCh37.Chr1, saPath, indexPath, preloadData.Positions, preloadData.PositionAlleleHashTable);
        }

        [Benchmark]
        public int XorFilter_5pct()
        {
            (string saPath, string indexPath) = Version5.Utilities.SaPath.GetPaths(_saDir, "0.05",
                Version5.Data.SaConstants.MaxCommonEntries, Version5.Data.SaConstants.MaxRareEntries);
            FileCacheBlaster.Blast(saPath, indexPath);

            VcfPreloadData preloadData = _preloadDataDict[NumSamples];
            return V5Preloader.Preload(GRCh37.Chr1, saPath, indexPath, preloadData.PositionAlleles, preloadData.PositionAlleleHashTable);
        }
    }
}