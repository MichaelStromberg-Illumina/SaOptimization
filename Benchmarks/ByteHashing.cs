using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using NirvanaCommon;

namespace Benchmarks
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    [MemoryDiagnoser]
    public class ByteHashing
    {
        private readonly string _inputString;
        private readonly byte[] _bytes;

        public ByteHashing()
        {
            _inputString = "abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq";
            _bytes       = Encoding.ASCII.GetBytes(_inputString);
        }

        [Benchmark(Baseline = true)]
        public int Current() => _inputString.GetHashCode();

        [Benchmark]
        public uint Murmur3() => Murmur.Hash32(_bytes);
    }
}