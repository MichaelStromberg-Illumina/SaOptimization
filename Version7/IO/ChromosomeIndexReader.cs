using System;
using System.Runtime.InteropServices;
using Compression.Data;
using NirvanaCommon;
using Version7.Data;

namespace Version7.IO
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

            int                 numBytes      = SpanBufferBinaryReader.ReadOptInt32(ref byteSpan);
            ReadOnlySpan<byte>  hashByteSpan  = SpanBufferBinaryReader.ReadBytes(ref byteSpan, numBytes);
            ReadOnlySpan<ulong> hashUlongSpan = MemoryMarshal.Cast<byte, ulong>(hashByteSpan);
            
            var commonHash = new LongHashTable();
            foreach (ulong value in hashUlongSpan) commonHash.Add(value);
            
            ulong seed      = SpanBufferBinaryReader.ReadOptUInt64(ref byteSpan);
            var   xorFilter = new Xor8(fingerprints, seed);
            
            IndexEntry[] commonEntries = ReadSection(ref byteSpan);
            IndexEntry[] rareEntries   = ReadSection(ref byteSpan);

            return new ChromosomeIndex(xorFilter, commonHash, commonEntries, rareEntries);
        }

        private static IndexEntry[] ReadSection(ref ReadOnlySpan<byte> byteSpan)
        {
            int numEntries = SpanBufferBinaryReader.ReadOptInt32(ref byteSpan);
            var entries    = new IndexEntry[numEntries];

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