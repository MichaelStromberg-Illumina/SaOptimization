using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using NirvanaCommon;

namespace Benchmarks
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    [MemoryDiagnoser]
    public class HashSets
    {
        private readonly long[] _input    = {10, 20, 30, 40, 20, 30, 30, 11};
        private readonly long[] _notAdded = {0, 12, 50};

        private const int ExpectedCount = 5;

        [Benchmark(Baseline = true)]
        public int HashSet()
        {
            var hashSet = new HashSet<long>();
            foreach (long input in _input) hashSet.Add(input);

            if (hashSet.Count != ExpectedCount) throw new InvalidDataException();

            foreach (long input in _input)
            {
                if (!hashSet.Contains(input)) throw new InvalidDataException();
            }

            foreach (long notAdded in _notAdded)
            {
                if (hashSet.Contains(notAdded)) throw new InvalidDataException();
            }

            return hashSet.Count;
        }

        [Benchmark]
        public int Long()
        {
            var hashSet = new LongHashTable();
            foreach (long input in _input) hashSet.Add(input);

            if (hashSet.Count != ExpectedCount) throw new InvalidDataException();

            foreach (long input in _input)
            {
                if (!hashSet.Contains(input)) throw new InvalidDataException();
            }

            foreach (long notAdded in _notAdded)
            {
                if (hashSet.Contains(notAdded)) throw new InvalidDataException();
            }

            return hashSet.Count;
        }
    }
}