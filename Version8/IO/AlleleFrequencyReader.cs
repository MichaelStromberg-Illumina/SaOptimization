using System;
using System.Collections.Generic;
using System.IO;
using Compression.Data;
using NirvanaCommon;
using Version8.Data;
using PreloadResult = Version8.Data.PreloadResult;

namespace Version8.IO
{
    public class AlleleFrequencyReader : IDisposable
    {
        private readonly Stream               _stream;
        private readonly ExtendedBinaryReader _reader;
        private readonly Block                _block;
        private readonly ZstdContext          _context;

        public AlleleFrequencyReader(Stream stream, Block block, ZstdContext context, bool leaveOpen = false)
        {
            _stream  = stream;
            _reader  = new ExtendedBinaryReader(stream, leaveOpen);
            _block   = block;
            _context = context;

            Header header = Header.Read(_reader);
            CheckHeader(header);
        }

        private static void CheckHeader(Header header)
        {
            if (header.Identifier != SaConstants.AlleleFrequencyIdentifier)
                throw new InvalidDataException(
                    $"Invalid index file. Identifier did not match: '{SaConstants.AlleleFrequencyIdentifier}");

            if (header.FileFormatVersion != AlleleFrequencyWriter.FileFormatVersion)
                throw new InvalidDataException(
                    $"Unsupported file format version (supported: {AlleleFrequencyWriter.FileFormatVersion} vs file: {header.FileFormatVersion}");
        }

        public List<PreloadResult> GetAnnotatedVariants(IndexEntry[] indexEntries, LongHashTable positionAlleleSet,
            int numPositions)
        {
            var results = new List<PreloadResult>(numPositions);

            foreach (IndexEntry indexEntry in indexEntries)
            {
                _stream.Position = indexEntry.Offset;

                _block.Read(_reader);
                _block.Decompress(_context);

                ReadOnlySpan<byte> byteSpan = _block.UncompressedBytes.AsSpan();

                int numEntries   = SpanBufferBinaryReader.ReadInt32(ref byteSpan);
                
                for (var entryIndex = 0; entryIndex < numEntries; entryIndex++)
                {
                    ulong positionAllele = SpanBufferBinaryReader.ReadUInt64(ref byteSpan);
                    int   rawRsId        = SpanBufferBinaryReader.ReadOptInt32(ref byteSpan);

                    if (positionAlleleSet.Contains(positionAllele))
                    {
                        results.Add(new PreloadResult(positionAllele, ConvertToRsId(rawRsId)));
                    }
                }
            }

            return results;
        }

        private static string ConvertToRsId(in int rawRsId)
        {
            throw new NotImplementedException();
        }

        public void Dispose() => _stream.Dispose();
    }
}