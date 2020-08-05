using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using NirvanaCommon;
using PreloadBaseline;
using Preloader;
using Version1;
using Version2;

namespace Benchmarks
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    [MemoryDiagnoser]
    public class PreloadingTN
    {
        private readonly List<int> _positions;

        public PreloadingTN() =>
            (_positions, _) = Preloader.Preloader.GetPositions(Datasets.PedigreeTsvPath);

        [Benchmark(Baseline = true)]
        public int Current() => Baseline.Preload(GRCh37.Chr1, _positions);

        [Benchmark]
        public int V1() => V1Preloader.Preload(GRCh37.Chr1, _positions, "0.05");

        [Benchmark]
        public int V2() => V2Preloader.Preload(GRCh37.Chr1, _positions);
    }
}