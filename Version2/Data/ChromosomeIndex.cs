using System;
using System.Collections.Generic;
using System.IO;
using Compression.Algorithms;
using Compression.Data;
using NirvanaCommon;
using Version2.IO;

namespace Version2.Data
{
    public sealed class ChromosomeIndex
    {
        public readonly BitArray     BitArray;
        public readonly IndexEntry[] Common;
        public readonly IndexEntry[] Rare;
        
        public ChromosomeIndex(BitArray bitArray, IndexEntry[] common, IndexEntry[] rare)
        {
            BitArray = bitArray;
            Common   = common;
            Rare     = rare;
        }

        public static ChromosomeIndex Read(ExtendedBinaryReader reader, Block block, ZstdContext context,
            ZstdDictionary dict)
        {
            block.Read(reader);
            block.Decompress(context, dict);
            
            var bufferReader = new BufferBinaryReader(block.UncompressedBytes);
            
            int    numBytes  = bufferReader.ReadOptInt32();
            byte[] byteArray = bufferReader.ReadBytes(numBytes);

            var intArray = new int[numBytes / sizeof(int)];
            Buffer.BlockCopy(byteArray, 0, intArray, 0, numBytes);
            
            var bitArray = new BitArray(intArray);

            IndexEntry[] commonEntries = ReadSection(bufferReader);
            IndexEntry[] rareEntries   = ReadSection(bufferReader);

            return new ChromosomeIndex(bitArray, commonEntries, rareEntries);
        }

        private static IndexEntry[] ReadSection(BufferBinaryReader reader)
        {
            int numEntries = reader.ReadOptInt32();
            var entries    = new IndexEntry[numEntries];

            var  prevEnd    = 0;
            long prevOffset = 0;

            for (var i = 0; i < numEntries; i++)
            {
                int  deltaPosition = reader.ReadOptInt32();
                long deltaOffset   = reader.ReadOptInt64();

                int  end    = prevEnd    + deltaPosition;
                long offset = prevOffset + deltaOffset;

                entries[i] = new IndexEntry(end, offset);

                prevEnd    = end;
                prevOffset = offset;
            }

            return entries;
        }

        public void Write(ExtendedBinaryWriter writer, ZstdContext context, ZstdDictionary dict)
        {
            byte[] bytes;
            int numBytes;
            
            using (var memoryStream = new MemoryStream())
            using (var memoryWriter = new ExtendedBinaryWriter(memoryStream))
            {
                int[] intArray  = BitArray.Data;
                var   byteArray = new byte[intArray.Length * sizeof(int)];
                Buffer.BlockCopy(intArray, 0, byteArray, 0, byteArray.Length);
                
                memoryWriter.WriteOpt(byteArray.Length);
                memoryWriter.Write(byteArray);
                WriteSection(memoryWriter, Common);
                WriteSection(memoryWriter, Rare);

                bytes    = memoryStream.ToArray();
                numBytes = (int) memoryStream.Position;
            }

            int compressedBufferSize = ZstandardDict.GetCompressedBufferBounds(numBytes);
            var compressedBytes      = new byte[compressedBufferSize];
            
            int numCompressedBytes = ZstandardDict.Compress(bytes, numBytes,
                compressedBytes, compressedBufferSize, context, dict);

            Console.WriteLine(
                $"ChromosomeIndex.Write: uncompressed: {numBytes:N0} bytes, compressed: {numCompressedBytes:N0} bytes");
            
            var block = new WriteBlock(compressedBytes, numCompressedBytes, numBytes, 0, 0);
            block.Write(writer);
        }

        private static void WriteSection(ExtendedBinaryWriter writer, IndexEntry[] entries)
        {
            writer.WriteOpt(entries.Length);

            var  prevEnd    = 0;
            long prevOffset = 0;

            foreach (IndexEntry entry in entries)
            {
                //Console.WriteLine($"- file offset: {entry.Offset:N0}, end: {entry.End:N0}");

                int  deltaPostion = entry.End    - prevEnd;
                long deltaOffset  = entry.Offset - prevOffset;

                writer.WriteOpt(deltaPostion);
                writer.WriteOpt(deltaOffset);

                prevEnd    = entry.End;
                prevOffset = entry.Offset;
            }
        }

        public IndexEntry[] GetIndexEntries(List<int> positions)
        {
            var commonEntries = new List<IndexEntry>();
            var rareEntries   = new List<IndexEntry>();

            int lastCommonBlockIndex = -1;
            int lastRareBlockIndex   = -1;

            foreach (int position in positions)
            {
                GetIndexEntry(position, Common, commonEntries, ref lastCommonBlockIndex);
                if (BitArray.Get(position)) GetIndexEntry(position, Rare, rareEntries, ref lastRareBlockIndex);
            }

            commonEntries.AddRange(rareEntries);
            return commonEntries.ToArray();
        }

        private static void GetIndexEntry(int position, IndexEntry[] index, List<IndexEntry> foundBlocks,
            ref int lastBlockIndex)
        {
            int blockIndex = BinarySearch(index, position);
            if (blockIndex < 0 || blockIndex == lastBlockIndex) return;

            foundBlocks.Add(index[blockIndex]);
            lastBlockIndex = blockIndex;
        }

        private static int BinarySearch(IndexEntry[] entries, int position)
        {
            int begin = 0;
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
    }
}