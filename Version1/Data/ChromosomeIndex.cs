using System;
using System.Collections.Generic;
using NirvanaCommon;
using VariantGrouping;

namespace Version1.Data
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

        public static ChromosomeIndex Read(ExtendedBinaryReader reader)
        {
            int    numBytes  = reader.ReadOptInt32();
            byte[] byteArray = reader.ReadBytes(numBytes);

            var intArray = new int[numBytes / sizeof(int)];
            
            //Console.WriteLine($"Read: byte array length: {byteArray.Length}, int array length: {intArray.Length}");
            
            Buffer.BlockCopy(byteArray, 0, intArray, 0, numBytes);
            
            var bitArray = new BitArray(intArray);

            IndexEntry[] commonEntries = ReadSection(reader);
            IndexEntry[] rareEntries   = ReadSection(reader);

            return new ChromosomeIndex(bitArray, commonEntries, rareEntries);
        }

        private static IndexEntry[] ReadSection(ExtendedBinaryReader reader)
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

        public void Write(ExtendedBinaryWriter writer)
        {
            int[] intArray  = BitArray.Data;
            var   byteArray = new byte[intArray.Length * sizeof(int)];
            Buffer.BlockCopy(intArray, 0, byteArray, 0, byteArray.Length);
            Console.WriteLine($"Write: int array length: {intArray.Length}, byte array length: {byteArray.Length}");

            writer.WriteOpt(byteArray.Length);
            writer.Write(byteArray);
            WriteSection(writer, Common);
            WriteSection(writer, Rare);
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

        public BlockRange[] GetBlockRanges(List<PreloadVariant> variants)
        {
            // Console.WriteLine("Common:");
            // for (int j = 0; j < Common.Length; j++)
            // {
            //     Console.WriteLine($"- index: {j}, offset: {Common[j].Offset:N0}, end: {Common[j].End:N0}");
            // }
            //
            //
            // Console.WriteLine("Rare:");
            // for (int j = 0; j < Rare.Length; j++)
            // {
            //     Console.WriteLine($"- index: {j}, end: {Rare[j].End:N0}");
            // }
            
            var commonBlockIndices = new List<int>();
            var rareBlockIndices   = new List<int>();

            int currentCommonBlockIndex = -1;
            int currentRareBlockIndex   = -1;
            
            // first stage
            foreach (PreloadVariant variant in variants)
            {
                // Console.WriteLine($"Position: {variant.Position:N0}:");
                bool isRare = BitArray.Get(variant.Position);
                
                int  commonBlockIndex = BinarySearch(Common, variant.Position, currentCommonBlockIndex);
                bool foundCommonBlock = commonBlockIndex >= 0;
                
                // Console.WriteLine($"- common block index: {commonBlockIndex}");

                if (foundCommonBlock && commonBlockIndex != currentCommonBlockIndex)
                {
                    // Console.WriteLine($"- adding common block index: {commonBlockIndex}");
                    commonBlockIndices.Add(commonBlockIndex);
                    currentCommonBlockIndex = commonBlockIndex;
                }

                if (isRare)
                {
                    int  rareBlockIndex = BinarySearch(Rare, variant.Position, currentRareBlockIndex);
                    bool foundRareBlock = rareBlockIndex >= 0;
                
                    // Console.WriteLine($"- rare block index: {rareBlockIndex}");
                
                    if (foundRareBlock && rareBlockIndex != currentRareBlockIndex)
                    {
                        // Console.WriteLine($"- adding rare block index: {rareBlockIndex}");
                        rareBlockIndices.Add(rareBlockIndex);
                        currentRareBlockIndex = rareBlockIndex;
                    }
                }
                
                // Console.WriteLine();
            }

            // Console.WriteLine("Common block indices:");
            // foreach (int blockIndex in commonBlockIndices)
            // {
            //     Console.WriteLine($"- {blockIndex:N0}");
            // }
            //
            // Console.WriteLine();
            //
            // Console.WriteLine("Rare block indices:");
            // foreach (int blockIndex in rareBlockIndices)
            // {
            //     Console.WriteLine($"- {blockIndex:N0}");
            // }

            var commonBlockRanges = GetBlockRangesFromIndices(commonBlockIndices, Common);

            // Console.WriteLine("Common block ranges:");
            // foreach (BlockRange blockRange in commonBlockRanges)
            // {
            //     Console.WriteLine($"- file offset: {blockRange.FileOffset:N0}, # of blocks: {blockRange.NumBlocks}");
            // }
            //
            // Console.WriteLine();
            
            var rareBlockRanges = GetBlockRangesFromIndices(rareBlockIndices, Rare);

            // Console.WriteLine("Rare block ranges:");
            // foreach (BlockRange blockRange in rareBlockRanges)
            // {
            //     Console.WriteLine($"- file offset: {blockRange.FileOffset:N0}, # of blocks: {blockRange.NumBlocks}");
            // }

            commonBlockRanges.AddRange(rareBlockRanges);
            return commonBlockRanges.ToArray();
        }

        private static int BinarySearch(IndexEntry[] entries, int position, int begin)
        {
            if (begin < 0) begin = 0;
            int end = entries.Length - 1;

            while (begin <= end)
            {
                int index = begin + (end - begin >> 1);
                // Console.WriteLine($"  - begin: {begin}, end: {end}, index: {index}");
                IndexEntry indexEntry = entries[index];

                int start = index == 0 ? 1 : entries[index - 1].End + 1;
                // Console.WriteLine($"  - index entry: [{start:N0} - {indexEntry.End:N0}]");
                
                if (position >= start && position <= indexEntry.End) return index;

                if (indexEntry.End < position) begin = index + 1;
                else if (position  < start) end      = index - 1;
            }

            return ~begin;
        }

        private static List<BlockRange> GetBlockRangesFromIndices(List<int> blockIndices, IndexEntry[] entries)
        {
            int numIndices  = blockIndices.Count;
            var blockRanges = new List<BlockRange>(numIndices);

            int i = 0;
            while (i < numIndices)
            {
                int j = i;

                while (j + 1 < numIndices && blockIndices[j + 1] == blockIndices[j] + 1) j++;

                if (i == j)
                {
                    blockRanges.Add(new BlockRange(entries[i].Offset, 1));
                    i++;
                }
                else
                {
                    blockRanges.Add(new BlockRange(entries[i].Offset, j - i + 1));
                    i = j + 1;
                }
            }

            return blockRanges;
        }
    }
}