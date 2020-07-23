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
    public class SubsamplePreloading
    {
        private const    int                    MaxSamples      = 100_000;
        private readonly List<int>[]            _positionsArray = new List<int>[MaxSamples            + 1];
        private readonly List<PreloadVariant>[] _variantsArray  = new List<PreloadVariant>[MaxSamples + 1];

        // public int[] NumSamplesValues => new[] {100_000, 10_000, 1_000, 100, 10, 1};
        public int[] NumSamplesValues => new[] {1};
        
        [ParamsSource(nameof(NumSamplesValues))]
        public int NumSamples;

        public SubsamplePreloading()
        {
            List<int> tempPositions = Preloader.Preloader.GetPositions(@"E:\Data\Nirvana\gnomAD_chr1_preload.tsv");

            foreach (var numSamples in NumSamplesValues)
            {
                List<int> positions = Subsampler.Subsample(tempPositions, numSamples);
                _positionsArray[numSamples] = positions;
                _variantsArray[numSamples]  = GetPreloadVariants(positions);
            }
        }

        private static List<PreloadVariant> GetPreloadVariants(List<int> positions)
        {
            var variants = new List<PreloadVariant>(positions.Count);
            foreach (int position in positions)
                variants.Add(new PreloadVariant(position, null, VariantType.SNV));
            return variants;
        }

        [Benchmark(Baseline = true)]
        public int Current() => Baseline.Preload(GRCh37.Chr1, _positionsArray[NumSamples]);

        [Benchmark]
        public int V1() => V1Preloader.Preload(GRCh37.Chr1, _variantsArray[NumSamples]);
        
        [Benchmark]
        public int V2() => V2Preloader.Preload(GRCh37.Chr1, _variantsArray[NumSamples]);
    }
}