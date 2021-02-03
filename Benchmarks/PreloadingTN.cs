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
    [RankColumn, MinColumn, MaxColumn]
    [MemoryDiagnoser]
    [MaxRelativeError(0.005)]
    public class PreloadingTN
    {
        private readonly VcfPreloadData _preloadData = PreloadedData.TumorNormalData;
        private readonly string _saDir = SupplementaryAnnotation.Directory;

        [Benchmark(Baseline = true)]
        public int Current()
        {
            (string saPath, string indexPath) = SaPath.GetPaths(SupplementaryAnnotation.DevelopDirectory);
            FileCacheBlaster.Blast(saPath, indexPath);
            return Baseline.Preload(GRCh37.Chr1, saPath, indexPath, _preloadData.Positions);
        }

        [Benchmark]
        public int RareBitVector_5pct()
        {
            (string saPath, string indexPath) = Version1.Utilities.SaPath.GetPaths(_saDir, "0.05");
            FileCacheBlaster.Blast(saPath, indexPath);
            return V1Preloader.Preload(GRCh37.Chr1, saPath, indexPath, _preloadData.Positions);
        }

        [Benchmark]
        public int TwoBitVectors_5pct()
        {
            (string saPath, string indexPath) = Version2.Utilities.SaPath.GetPaths(_saDir);
            FileCacheBlaster.Blast(saPath, indexPath);
            return V2Preloader.Preload(GRCh37.Chr1, saPath, indexPath, _preloadData.Positions);
        }
        
        [Benchmark]
        public int NoBitVector_5pct()
        {
            (string saPath, string indexPath) = Version3.Utilities.SaPath.GetPaths(_saDir);
            FileCacheBlaster.Blast(saPath, indexPath);
            return V3Preloader.Preload(GRCh37.Chr1, saPath, indexPath, _preloadData.Positions);
        }
        
        [Benchmark]
        public int RareBitVector_Span_AI_5pct()
        {
            (string saPath, string indexPath) = Version4.Utilities.SaPath.GetPaths(_saDir);
            FileCacheBlaster.Blast(saPath, indexPath);
            return V4Preloader.Preload(GRCh37.Chr1, saPath, indexPath, _preloadData.Positions,
                _preloadData.PositionAlleleHashTable);
        }
        
        [Benchmark]
        public int XorFilter_5pct()
        {
            (string saPath, string indexPath) = Version5.Utilities.SaPath.GetPaths(_saDir, "0.05",
                Version5.Data.SaConstants.MaxCommonEntries, Version5.Data.SaConstants.MaxRareEntries);
            FileCacheBlaster.Blast(saPath, indexPath);
            
            return V5Preloader.Preload(GRCh37.Chr1, saPath, indexPath, _preloadData.PositionAlleles,
                _preloadData.PositionAlleleHashTable);
        }
    }
}