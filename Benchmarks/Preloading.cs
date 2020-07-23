using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using NirvanaCommon;
using PreloadBaseline;
using VariantGrouping;
using Version1;
using Version2;

namespace Benchmarks
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    [MemoryDiagnoser]
    public class Preloading
    {
        private readonly List<int> _positions;
        private readonly List<PreloadVariant> _variants;

        public Preloading()
        {
            _positions = Preloader.Preloader.GetPositions(@"E:\Data\Nirvana\gnomAD_chr1_preload.tsv");
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
        public int Current() => Baseline.Preload(GRCh37.Chr1, _positions);

        [Benchmark]
        public int V1() => V1Preloader.Preload(GRCh37.Chr1, _variants);
        
        [Benchmark]
        public int V2() => V2Preloader.Preload(GRCh37.Chr1, _variants);
    }
}