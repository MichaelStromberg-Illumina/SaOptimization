using System;
using System.Collections.Generic;
using System.IO;
using Compression.Data;
using NirvanaCommon;
using Version7.Data;
using PreloadResult = Version7.Data.PreloadResult;

namespace Version7.IO
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
            var gnomad = new GnomadReadEntry();

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
                    gnomad.Read(ref byteSpan);

                    if (positionAlleleSet.Contains(positionAllele))
                    {
                        // string json = GnomadJson.GetJson(gnomad);
                        results.Add(new PreloadResult(positionAllele, gnomad.Clone()));
                    }
                }
            }

            return results;
        }

        public void Dispose() => _stream.Dispose();
    }
}