using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Benchmarks.Data;
using NirvanaCommon;
using Preloader;
using Version5;

namespace Benchmarks
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn, MinColumn, MaxColumn]
    [MemoryDiagnoser]
    [MaxRelativeError(0.005)]
    public class PreloadingBlockSizes
    {
        private readonly VcfPreloadData _preloadData = PreloadedData.PedigreeData;
        private readonly string         _saDir       = SupplementaryAnnotation.Directory;

        private const string threshold = "0.07";

        [Params(1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072, 262144)]
        public int Common { get; set; }

        [Params(8, 16, 32, 64, 128, 256, 512)]
        public int Rare { get; set; }

        [IterationSetup]
        public void Setup()
        {
            (string saPath, string indexPath) = Version5.Utilities.SaPath.GetPaths(_saDir, threshold, Common, Rare);
            FileCacheBlaster.Blast(saPath, indexPath);
        }
        
        [Benchmark]
        public int XorFilter_7pct()
        {
            (string saPath, string indexPath) = Version5.Utilities.SaPath.GetPaths(_saDir, threshold, Common, Rare);
            int numPreloadedVariants = V5Preloader.Preload(GRCh37.Chr1, saPath, indexPath, _preloadData.PositionAlleles,
                _preloadData.PositionAlleleHashTable);
            if (numPreloadedVariants != Datasets.NumPedigreePreloadedVariants) throw new InvalidDataException();
            return numPreloadedVariants;
        }
    }
}