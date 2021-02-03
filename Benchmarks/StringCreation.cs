using System;
using System.Buffers;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Benchmarks.StringAlgorithms;

namespace Benchmarks
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    [MemoryDiagnoser]
    public class StringCreation
    {
        private readonly byte[] _buffer;
        private readonly byte[] _utf8Buffer;

        private readonly Decoder _decoder = Encoding.UTF8.GetDecoder();

        public StringCreation()
        {
            const string s = "Michael is the best!";
            _buffer     = Encoding.ASCII.GetBytes(s);
            _utf8Buffer = Encoding.UTF8.GetBytes(s);
        }

        [Benchmark(Baseline = true)]
        public string ASCII() => Encoding.ASCII.GetString(_buffer);

        [Benchmark]
        public string SIMD() => StringFun.ByteArrayToString(_buffer);
        
        [Benchmark]
        public string UTF8() => Encoding.UTF8.GetString(_utf8Buffer);

        [Benchmark]
        public string SpanBuffer()
        {
            int numBytes = _utf8Buffer.Length;
            Span<byte> byteSpan = _utf8Buffer.AsSpan();

            int maxBufferSize = Encoding.UTF8.GetMaxCharCount(numBytes);

            char[] charBuffer = ArrayPool<char>.Shared.Rent(maxBufferSize);

            Span<char> charSpan = charBuffer.AsSpan();
            int numChars = _decoder.GetChars(byteSpan.Slice(0, numBytes), charSpan, true);
            charSpan = charSpan.Slice(0, numChars);
            
            var ret = new string(charSpan);
            ArrayPool<char>.Shared.Return(charBuffer);
            return ret;
        }
    }
}