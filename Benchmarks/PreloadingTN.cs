using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Benchmarks.Data;
using NirvanaCommon;
using PreloadBaseline;
using Version1;
using Version2;

namespace Benchmarks
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    [MemoryDiagnoser]
    public class PreloadingTN
    {
        private readonly VcfPreloadData _preloadData = PreloadedData.TumorNormalData;

        [Benchmark(Baseline = true)]
        public int Current() => Baseline.Preload(GRCh37.Chr1, _preloadData.Positions);

        [Benchmark]
        public int V1() => V1Preloader.Preload(GRCh37.Chr1, _preloadData.Positions, "0.05");

        [Benchmark]
        public int V2() => V2Preloader.Preload(GRCh37.Chr1, _preloadData.Positions);
    }
}