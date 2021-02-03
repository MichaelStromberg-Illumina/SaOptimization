using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Benchmarks.Data;
using NirvanaCommon;
using PreloadBaseline;
using Preloader;
using Version4;
using Version5;
using Version6;

namespace Benchmarks
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn, MinColumn, MaxColumn]
    [MemoryDiagnoser]
    [MaxRelativeError(0.005)]
    public class Preloading
    {
        private readonly VcfPreloadData _preloadData = PreloadedData.PedigreeData;
        private readonly string         _saDir       = SupplementaryAnnotation.Directory;

        [Benchmark(Baseline = true)]
        public int Current()
        {
            (string saPath, string indexPath) = SaPath.GetPaths(SupplementaryAnnotation.DevelopDirectory);
            FileCacheBlaster.Blast(saPath, indexPath);
            
            int numPreloadedVariants = Baseline.Preload(GRCh37.Chr1, saPath, indexPath, _preloadData.Positions);
            if (numPreloadedVariants != Datasets.NumPedigreePreloadedPositionalVariants)
                throw new InvalidDataException();
            return numPreloadedVariants;
        }
        
        // [Benchmark]
        // public int RareBitVector_5pct()
        // {
        //     (string saPath, string indexPath) = Version1.Utilities.SaPath.GetPaths(_saDir, "0.05");
        //     FileCacheBlaster.Blast(saPath, indexPath);
        //     
        //     int numPreloadedVariants = V1Preloader.Preload(GRCh37.Chr1, saPath, indexPath, _preloadData.Positions);
        //     if (numPreloadedVariants != Datasets.NumPedigreePreloadedPositionalVariants)
        //         throw new InvalidDataException();
        //     return numPreloadedVariants;
        // }
        //
        // [Benchmark]
        // public int TwoBitVectors_5pct()
        // {
        //     (string saPath, string indexPath) = Version2.Utilities.SaPath.GetPaths(_saDir);
        //     FileCacheBlaster.Blast(saPath, indexPath);
        //     
        //     int numPreloadedVariants = V2Preloader.Preload(GRCh37.Chr1, saPath, indexPath, _preloadData.Positions);
        //     if (numPreloadedVariants != Datasets.NumPedigreePreloadedPositionalVariants)
        //         throw new InvalidDataException();
        //     return numPreloadedVariants;
        // }
        //
        // [Benchmark]
        // public int NoBitVector_5pct()
        // {
        //     (string saPath, string indexPath) = Version3.Utilities.SaPath.GetPaths(_saDir);
        //     FileCacheBlaster.Blast(saPath, indexPath);
        //     
        //     int numPreloadedVariants = V3Preloader.Preload(GRCh37.Chr1, saPath, indexPath, _preloadData.Positions);
        //     if (numPreloadedVariants != Datasets.NumPedigreePreloadedPositionalVariants)
        //         throw new InvalidDataException();
        //     return numPreloadedVariants;
        // }
        
        [Benchmark]
        public int RareBitVector_Span_AI_5pct()
        {
            (string saPath, string indexPath) = Version4.Utilities.SaPath.GetPaths(_saDir);
            FileCacheBlaster.Blast(saPath, indexPath);
            
            int numPreloadedVariants =
                V4Preloader.Preload(GRCh37.Chr1, saPath, indexPath, _preloadData.Positions, _preloadData.PositionAlleleHashTable);
            if (numPreloadedVariants != Datasets.NumPedigreePreloadedVariants) throw new InvalidDataException();
            return numPreloadedVariants;
        }

        [Benchmark]
        public int XorFilter_5pct()
        {
            (string saPath, string indexPath) = Version5.Utilities.SaPath.GetPaths(_saDir, "0.05",
                Version5.Data.SaConstants.MaxCommonEntries, Version5.Data.SaConstants.MaxRareEntries);
            FileCacheBlaster.Blast(saPath, indexPath);
            
            int numPreloadedVariants = V5Preloader.Preload(GRCh37.Chr1, saPath, indexPath, _preloadData.PositionAlleles,
                _preloadData.PositionAlleleHashTable);
            if (numPreloadedVariants != Datasets.NumPedigreePreloadedVariants) throw new InvalidDataException();
            return numPreloadedVariants;
        }

        [Benchmark]
        public int XorFilter_7pct()
        {
            (string saPath, string indexPath) = Version5.Utilities.SaPath.GetPaths(_saDir, "0.05",
                Version5.Data.SaConstants.MaxCommonEntries, Version5.Data.SaConstants.MaxRareEntries);
            FileCacheBlaster.Blast(saPath, indexPath);
            
            int numPreloadedVariants = V5Preloader.Preload(GRCh37.Chr1, saPath, indexPath, _preloadData.PositionAlleles,
                _preloadData.PositionAlleleHashTable);
            if (numPreloadedVariants != Datasets.NumPedigreePreloadedVariants) throw new InvalidDataException();
            return numPreloadedVariants;
        }
        
        [Benchmark]
        public int XorFilter_NoAI_5pct()
        {
            (string saPath, string indexPath) = Version6.Utilities.SaPath.GetPaths(_saDir, "0.05",
                Version5.Data.SaConstants.MaxCommonEntries, Version5.Data.SaConstants.MaxRareEntries);
            FileCacheBlaster.Blast(saPath, indexPath);
            
            int numPreloadedVariants = V6Preloader.Preload(GRCh37.Chr1, saPath, indexPath, _preloadData.PositionAlleles,
                _preloadData.PositionAlleleHashTable);
            if (numPreloadedVariants != Datasets.NumPedigreePreloadedVariants) throw new InvalidDataException();
            return numPreloadedVariants;
        }
    }
}