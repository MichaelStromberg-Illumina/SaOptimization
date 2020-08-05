using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Compression.Data;
using NirvanaCommon;
using Version3.Data;

namespace Version3.IO
{
    public class AlleleFrequencyReader : IDisposable
    {
        private readonly Stream               _stream;
        private readonly ExtendedBinaryReader _reader;
        private readonly Block                _block;
        private readonly ZstdContext          _context;

        private readonly ZstdDictionary _dictionary;
        public ZstdDictionary Dictionary => _dictionary;

        public AlleleFrequencyReader(Stream stream, Block block, ZstdContext context, bool leaveOpen = false)
        {
            _stream  = stream;
            _reader  = new ExtendedBinaryReader(stream, leaveOpen);
            _block   = block;
            _context = context;

            Header header = Header.Read(_reader);
            CheckHeader(header);

            _dictionary = new ZstdDictionary(CompressionMode.Decompress, header.CompressionDictionary, 17);
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

        public List<PreloadResult> GetAnnotatedVariants(IndexEntry[] indexEntries, BitArray preloadBitArray,
            int numPositions)
        {
            var results = new List<PreloadResult>(numPositions);

            foreach (IndexEntry indexEntry in indexEntries)
            {
                _stream.Position = indexEntry.Offset;

                _block.Read(_reader);
                _block.DecompressDict(_context, _dictionary);

                var reader       = new BufferBinaryReader(_block.UncompressedBytes);
                int numEntries   = reader.ReadOptInt32();
                var lastPosition = 0;

                for (var entryIndex = 0; entryIndex < numEntries; entryIndex++)
                {
                    int position = reader.ReadOptInt32() + lastPosition;

                    if (preloadBitArray.Get(position))
                    {
                        var    variantType = (VariantType) reader.ReadByte();
                        string allele      = reader.ReadString();
                        string json        = reader.ReadString();

                        results.Add(new PreloadResult(position, null, allele, json));
                    }
                    else
                    {
                        reader.SkipByte();
                        reader.SkipString();
                        reader.SkipString();
                    }

                    lastPosition = position;
                }
            }

            return results;
        }

        public void Dispose() => _stream.Dispose();
    }
}