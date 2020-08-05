using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Benchmarks.BloomFilterAlgorithms;
using NirvanaCommon;

namespace Benchmarks
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    [MemoryDiagnoser]
    public class BloomFilters
    {
        private readonly BloomFilter    _bloomFilter;
        private readonly BloomFilterOpt _bloomFilterOpt;
        private readonly Xor8           _xor8;
        private readonly Xor16          _xor16;
        private readonly HashSet<ulong> _hashSet;

        private const double TargetErrorRate = 0.003906;
        private const int    MaxKey          = 100_000_000;

        public BloomFilters()
        {
            var keys = new HashSet<ulong>
            {
                1,
                10,
                100,
                1000,
                10000,
                100000,
                1000000,
                10000000,
                100000000,
            };

            _bloomFilter = new BloomFilter(keys.Count, TargetErrorRate);
            foreach (ulong key in keys) _bloomFilter.Add(key);
            
            _bloomFilterOpt = new BloomFilterOpt(keys.Count, TargetErrorRate);
            foreach (ulong key in keys) _bloomFilterOpt.Add(key);

            _xor8  = NirvanaCommon.Xor8.Construction(keys.ToArray());
            _xor16 = NirvanaCommon.Xor16.Construction(keys.ToArray());

            _hashSet = new HashSet<ulong>(keys.Count);
            foreach (ulong key in keys) _hashSet.Add(key);
        }

        [Benchmark(Baseline = true)]
        public int HashSet()
        {
            var numPositive = 0;
            for (ulong i = 1; i <= MaxKey; i++)
                if (_hashSet.Contains(i))
                    numPositive++;
            return numPositive;
        }
        
        [Benchmark]
        public int BloomFilter()
        {
            var numPositive = 0;
            for (ulong i = 1; i <= MaxKey; i++)
                if (_bloomFilter.Contains(i))
                    numPositive++;
            return numPositive;
        }

        [Benchmark]
        public int BloomFilterOpt()
        {
            var numPositive = 0;
            for (ulong i = 1; i <= MaxKey; i++)
                if (_bloomFilterOpt.Contains(i))
                    numPositive++;
            return numPositive;
        }

        [Benchmark]
        public int Xor8()
        {
            var numPositive = 0;
            for (ulong i = 1; i <= MaxKey; i++)
                if (_xor8.Contains(i))
                    numPositive++;
            return numPositive;
        }

        [Benchmark]
        public int Xor16()
        {
            var numPositive = 0;
            for (ulong i = 1; i <= MaxKey; i++)
                if (_xor16.Contains(i))
                    numPositive++;
            return numPositive;
        }
    }
}