using System;
using System.Runtime.InteropServices;
using Compression.Data;
using NirvanaCommon;
using Version5.Data;

namespace Version5.IO
{
    public static class ChromosomeIndexReader
    {
        public static ChromosomeIndex Read(ExtendedBinaryReader reader, Block block, ZstdContext context)
        {
            block.Read(reader);
            block.Decompress(context);
            
            ReadOnlySpan<byte> byteSpan = block.UncompressedBytes.AsSpan();

            int    numFingerprints = SpanBufferBinaryReader.ReadOptInt32(ref byteSpan);
            byte[] fingerprints    = SpanBufferBinaryReader.ReadBytes(ref byteSpan, numFingerprints).ToArray();
            // Console.WriteLine($"- # of fingerprints: {numFingerprints:N0}");

            int                 numBytes      = SpanBufferBinaryReader.ReadOptInt32(ref byteSpan);
            ReadOnlySpan<byte>  hashByteSpan  = SpanBufferBinaryReader.ReadBytes(ref byteSpan, numBytes);
            ReadOnlySpan<ulong> hashUlongSpan = MemoryMarshal.Cast<byte, ulong>(hashByteSpan);
            // Console.WriteLine($"- # of hash table bytes: {numBytes:N0}, ulongs: {hashUlongSpan.Length:N0}");
            
            var commonHash = new LongHashTable();
            foreach (ulong value in hashUlongSpan)
            {
                // if (value == 1002626412615658) Console.WriteLine("commonHash: 14590-G-A (rare)");
                // if (value == 1008605007091690) Console.WriteLine("commonHash: 14677-G-A (rare)");
                commonHash.Add(value);
            }
            
            ulong seed      = SpanBufferBinaryReader.ReadOptUInt64(ref byteSpan);
            var   xorFilter = new Xor8(fingerprints, seed);
            // Console.WriteLine($"- seed: {seed:N0}");
            
            long alleleIndexOffset = SpanBufferBinaryReader.ReadOptInt64(ref byteSpan);
            // Console.WriteLine($"- allele index offset: {alleleIndexOffset:N0}");
            
            IndexEntry[] commonEntries = ReadSection(ref byteSpan);
            IndexEntry[] rareEntries   = ReadSection(ref byteSpan);

            return new ChromosomeIndex(xorFilter, commonHash, commonEntries, rareEntries, alleleIndexOffset);
        }

        private static IndexEntry[] ReadSection(ref ReadOnlySpan<byte> byteSpan)
        {
            int numEntries = SpanBufferBinaryReader.ReadOptInt32(ref byteSpan);
            var entries    = new IndexEntry[numEntries];
            // Console.WriteLine($"- # section entries: {numEntries:N0}");

            var  prevEnd    = 0;
            long prevOffset = 0;

            for (var i = 0; i < numEntries; i++)
            {
                int  deltaPosition = SpanBufferBinaryReader.ReadOptInt32(ref byteSpan);
                long deltaOffset   = SpanBufferBinaryReader.ReadOptInt64(ref byteSpan);

                int  end    = prevEnd    + deltaPosition;
                long offset = prevOffset + deltaOffset;

                entries[i] = new IndexEntry(end, offset);

                prevEnd    = end;
                prevOffset = offset;
            }

            return entries;
        }
    }
}