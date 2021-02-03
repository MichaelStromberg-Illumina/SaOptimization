using System;
using System.IO;
using System.Runtime.InteropServices;
using Compression.Algorithms;
using Compression.Data;
using NirvanaCommon;
using Version7.Data;

namespace Version7.IO
{
    public static class ChromosomeIndexWriter
    {
        public static void Write(this ChromosomeIndex index, ExtendedBinaryWriter writer, ZstdContext context)
        {
            byte[] bytes;
            int    numBytes;

            using (var memoryStream = new MemoryStream())
            using (var memoryWriter = new ExtendedBinaryWriter(memoryStream))
            {
                ulong[]             commonEntries = index.CommonPositionAlleles.GetData();
                ReadOnlySpan<ulong> ulongSpan     = commonEntries.AsSpan();
                ReadOnlySpan<byte>  byteSpan      = MemoryMarshal.Cast<ulong, byte>(ulongSpan);

                memoryWriter.WriteOpt(index.XorFilter.Data.Length);
                memoryWriter.Write(index.XorFilter.Data);
                memoryWriter.WriteOpt(byteSpan.Length);
                memoryWriter.Write(byteSpan);
                memoryWriter.WriteOpt(index.XorFilter.Seed);

                WriteSection(memoryWriter, index.Common);
                WriteSection(memoryWriter, index.Rare);

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
    }
}