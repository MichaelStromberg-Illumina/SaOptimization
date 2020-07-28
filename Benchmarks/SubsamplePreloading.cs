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
    public class SubsamplePreloading
    {
        private const    int         MaxSamples      = 100_000;
        private readonly List<int>[] _positionsArray = new List<int>[MaxSamples + 1];

        public int[] NumSamplesValues => new[] {100_000, 10_000, 1_000, 100, 10, 1};
        
        [ParamsSource(nameof(NumSamplesValues))]
        // ReSharper disable once UnassignedField.Global
        public int NumSamples;

        public SubsamplePreloading()
        {
            List<int> tempPositions = Preloader.Preloader.GetPositions(Datasets.PedigreePreloadPath);

            foreach (int numSamples in NumSamplesValues)
            {
                _positionsArray[numSamples] = Subsampler.Subsample(tempPositions, numSamples);
            }
        }

        [Benchmark(Baseline = true)]
        public int Current() => Baseline.Preload(GRCh37.Chr1, _positionsArray[NumSamples]);

        [Benchmark]
        public int V1() => V1Preloader.Preload(GRCh37.Chr1, _positionsArray[NumSamples], "0.05");
        
        [Benchmark]
        public int V2() => V2Preloader.Preload(GRCh37.Chr1, _positionsArray[NumSamples]);
    }
}