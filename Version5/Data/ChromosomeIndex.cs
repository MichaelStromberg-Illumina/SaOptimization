using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Compression.Algorithms;
using Compression.Data;
using NirvanaCommon;
using Version5.IO;

namespace Version5.Data
{
    public sealed class ChromosomeIndex
    {
        public readonly Xor8          XorFilter;
        public readonly LongHashTable CommonPositionAlleles;
        public readonly IndexEntry[]  Common;
        public readonly IndexEntry[]  Rare;
        public readonly long          AlleleIndexOffset;

        public ChromosomeIndex(Xor8 xorFilter, LongHashTable commonHash, IndexEntry[] common, IndexEntry[] rare, long alleleIndexOffset)
        {
            XorFilter             = xorFilter;
            CommonPositionAlleles = commonHash;
            Common                = common;
            Rare                  = rare;
            AlleleIndexOffset     = alleleIndexOffset;
        }

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
            foreach (ulong value in hashUlongSpan) commonHash.Add(value);
            
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

        public void Write(ExtendedBinaryWriter writer, ZstdContext context)
        {
            byte[] bytes;
            int    numBytes;
            
            using (var memoryStream = new MemoryStream())
            using (var memoryWriter = new ExtendedBinaryWriter(memoryStream))
            {
                ulong[]             commonEntries = CommonPositionAlleles.GetData();
                ReadOnlySpan<ulong> ulongSpan     = commonEntries.AsSpan();
                ReadOnlySpan<byte>  byteSpan      = MemoryMarshal.Cast<ulong, byte>(ulongSpan);

                memoryWriter.WriteOpt(XorFilter.Data.Length);
                memoryWriter.Write(XorFilter.Data);
                memoryWriter.WriteOpt(byteSpan.Length);
                memoryWriter.Write(byteSpan);
                memoryWriter.WriteOpt(XorFilter.Seed);
                
                memoryWriter.WriteOpt(AlleleIndexOffset);
                WriteSection(memoryWriter, Common);
                WriteSection(memoryWriter, Rare);

                bytes    = memoryStream.ToArray();
                numBytes = (int) memoryStream.Position;
            }
            
            WriteBlock block = GetWriteBlock(bytes, numBytes, context);
            block.Write(writer);
        }

        private static WriteBlock GetWriteBlock(byte[] bytes, int numBytes, ZstdContext context)
        {
            int compressedBufferSize = ZstandardStatic.GetCompressedBufferBounds(numBytes);
            var compressedBytes      = new byte[compressedBufferSize];

            int numCompressedBytes = ZstandardStatic.Compress(bytes, numBytes,
                compressedBytes, compressedBufferSize, context);

            return new WriteBlock(compressedBytes, numCompressedBytes, numBytes, 0, 0);
        }

        private static void WriteSection(ExtendedBinaryWriter writer, IndexEntry[] entries)
        {
            writer.WriteOpt(entries.Length);

            var  prevEnd    = 0;
            long prevOffset = 0;

            foreach (IndexEntry entry in entries)
            {
                int  deltaPostion = entry.End    - prevEnd;
                long deltaOffset  = entry.Offset - prevOffset;

                writer.WriteOpt(deltaPostion);
                writer.WriteOpt(deltaOffset);

                prevEnd    = entry.End;
                prevOffset = entry.Offset;
            }
        }

        public IndexEntry[] GetIndexEntries(List<ulong> positionAlleles)
        {
            var entries = new List<IndexEntry>();

            var commonIndexes = new HashSet<int>();
            var rareIndexes   = new HashSet<int>();

            foreach (ulong positionAllele in positionAlleles)
            {
                int position = PositionAllele.GetPosition(positionAllele);

                if (CommonPositionAlleles.Contains(positionAllele)) GetIndexEntry(position, Common, commonIndexes);
                else GetIndexEntry(position,                                                Rare,   rareIndexes);
            }

            foreach (int index in commonIndexes.OrderBy(x => x)) entries.Add(Common[index]);
            foreach (int index in rareIndexes.OrderBy(x => x)) entries.Add(Rare[index]);

            // Console.WriteLine($"- GetIndexEntries: common index entries: {commonIndexes.Count:N0}, rare index entries: {rareIndexes.Count:N0}");
            
            return entries.ToArray();
        }

        private static void GetIndexEntry(int position, IndexEntry[] index, HashSet<int> hashSet)
        {
            int blockIndex = BinarySearch(index, position);
            if (blockIndex >= 0) hashSet.Add(blockIndex);
        }

        private static int BinarySearch(IndexEntry[] entries, int position)
        {
            var begin = 0;
            int end = entries.Length - 1;

            while (begin <= end)
            {
                int index = begin + (end - begin >> 1);
                IndexEntry indexEntry = entries[index];

                int start = index == 0 ? 1 : entries[index - 1].End + 1;
                if (position >= start && position <= indexEntry.End) return index;

                if (indexEntry.End < position) begin = index + 1;
                else if (position  < start) end      = index - 1;
            }

            return ~begin;
        }

        public List<ulong> Filter(ulong[] positionAlleles)
        {
            var filteredPositions = new List<ulong>(positionAlleles.Length);

            foreach (ulong pa in positionAlleles)
                if (XorFilter.Contains(pa))
                    filteredPositions.Add(pa);

            return filteredPositions;
        }
    }
}