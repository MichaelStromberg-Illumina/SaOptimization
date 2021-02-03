using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Benchmarks.VlqAlgorithms;
using NirvanaCommon;
using Version4.IO;

namespace Benchmarks
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    [MemoryDiagnoser]
    public class VLQ
    {
        private readonly byte[] _buffer;

        public VLQ()
        {
            const int a = 1_234_567_890;
            const int b = 1_234_567;
            const int c = 1_234;
            const int d = 6;

            using (var ms = new MemoryStream())
            using (var writer = new ExtendedBinaryWriter(ms))
            {
                writer.WriteOpt(a);
                writer.WriteOpt(b);
                writer.WriteOpt(c);
                writer.WriteOpt(d);
                _buffer = ms.ToArray();
            }
        }

        [Benchmark]
        public int SpanBuffer()
        {
            ReadOnlySpan<byte> byteSpan = _buffer.AsSpan();

            int ret = SpanReader.Version1(ref byteSpan);
            ret ^= SpanReader.Version1(ref byteSpan);
            ret ^= SpanReader.Version1(ref byteSpan);
            ret ^= SpanReader.Version1(ref byteSpan);
            return ret;
        }

        [Benchmark]
        public int SpanBuffer2()
        {
            ReadOnlySpan<byte> byteSpan = _buffer.AsSpan();
        
            int ret = SpanReader.Version2(ref byteSpan);
            ret ^= SpanReader.Version2(ref byteSpan);
            ret ^= SpanReader.Version2(ref byteSpan);
            ret ^= SpanReader.Version2(ref byteSpan);
            return ret;
        }
        
        [Benchmark]
        public int SpanBuffer3()
        {
            ReadOnlySpan<byte> byteSpan = _buffer.AsSpan();

            int ret = SpanReader.Version3(ref byteSpan);
            ret ^= SpanReader.Version3(ref byteSpan);
            ret ^= SpanReader.Version3(ref byteSpan);
            ret ^= SpanReader.Version3(ref byteSpan);
            return ret;
        }
        
        [Benchmark]
        public int Buffer()
        {
            var bufferReader = new BufferBinaryReader(_buffer);
        
            int ret = bufferReader.ReadOptInt32();
            ret ^= bufferReader.ReadOptInt32();
            ret ^= bufferReader.ReadOptInt32();
            ret ^= bufferReader.ReadOptInt32();
            return ret;
        }

        [Benchmark(Baseline = true)]
        public int MemoryStream()
        {
            int ret;
        
            using (var ms = new MemoryStream(_buffer))
            using (var reader = new ExtendedBinaryReader(ms))
            {
                ret =  reader.ReadOptInt32();
                ret ^= reader.ReadOptInt32();
                ret ^= reader.ReadOptInt32();
                ret ^= reader.ReadOptInt32();
            }
        
            return ret;
        }
    }
}