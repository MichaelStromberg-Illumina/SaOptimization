using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Benchmarks.Data;
using NirvanaCommon;
using Preloader;
using Version5;
using Version5.Data;

namespace Benchmarks
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn, MinColumn, MaxColumn]
    [MemoryDiagnoser]
    [MaxRelativeError(0.005)]
    public class PreloadingThreshold
    {
        private readonly VcfPreloadData _preloadData = PreloadedData.PedigreeData;
        private readonly string         _saDir       = SupplementaryAnnotation.Directory;

        // [Params("0.01", "0.02", "0.03", "0.04", "0.05", "0.06", "0.07", "0.08", "0.09", "0.10")]
        [Params("0.01", "0.02", "0.03", "0.04", "0.05", "0.06", "0.07", "0.08", "0.09", "0.10")]
        public string Threshold { get; set; }

        [IterationSetup]
        public void Setup()
        {
            (string saPath, string indexPath) = Version5.Utilities.SaPath.GetPaths(_saDir, Threshold,
                SaConstants.MaxCommonEntries, SaConstants.MaxRareEntries);
            FileCacheBlaster.Blast(saPath, indexPath);
        }

        [Benchmark]
        public int XorFilter()
        {
            (string saPath, string indexPath) = Version5.Utilities.SaPath.GetPaths(_saDir, Threshold,
                SaConstants.MaxCommonEntries, SaConstants.MaxRareEntries);
            int numPreloadedVariants = V5Preloader.Preload(GRCh37.Chr1, saPath, indexPath, _preloadData.PositionAlleles,
                _preloadData.PositionAlleleHashTable);
            if (numPreloadedVariants != Datasets.NumPedigreePreloadedVariants) throw new InvalidDataException();
            return numPreloadedVariants;
        }
    }
}