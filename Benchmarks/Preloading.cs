using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using PreloadBaseline;
using PreloadBaseline.Nirvana;

namespace Benchmarks
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn()]
    [MemoryDiagnoser]
    public class PreloadBaseline
    {
        private readonly IChromosome _chr1;
        private readonly List<int> _positions;

        public PreloadBaseline()
        {
            _chr1      = new Chromosome("chr1", "1", null, null, 0, 0);
            _positions = Preloader.Preloader.GetPositions();
        }

        [Benchmark(Baseline = true)]
        public int Current() => Baseline.Preload(_chr1, _positions);
    }
}