using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using NirvanaCommon;

namespace Benchmarks
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    [MemoryDiagnoser]
    public class StringHashing
    {
        private readonly string _inputString;

        public StringHashing()
        {
            _inputString = "abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq";
        }

        [Benchmark(Baseline = true)]
        public int Current() => _inputString.GetHashCode();

        [Benchmark]
        public uint Murmur3() => Murmur.String(_inputString);
    }
}