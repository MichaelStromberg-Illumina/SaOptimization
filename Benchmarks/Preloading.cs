using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using NirvanaCommon;
using PreloadBaseline;
using VariantGrouping;
using Version1;

namespace Benchmarks
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    [MemoryDiagnoser]
    public class Preloading
    {
        private readonly List<int> _positions;
        private readonly List<PreloadVariant> _variants;

        private const int ExpectedPreloadedVariants = 304_636;

        public Preloading()
        {
            _positions = Preloader.Preloader.GetPositions(@"E:\Data\Nirvana\gnomAD_chr1_pedigree_position_new.txt");
            _variants = GetPreloadVariants(_positions);
        }

        private static List<PreloadVariant> GetPreloadVariants(List<int> positions)
        {
            var variants = new List<PreloadVariant>(positions.Count);
            foreach (int position in positions)
                variants.Add(new PreloadVariant(position, null, VariantType.SNV));
            return variants;
        }

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

        // [Benchmark]
        // public int V2() => V2Preloader.Preload(GRCh37.Chr1, _variants);
    }
}