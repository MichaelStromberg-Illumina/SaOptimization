using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Compression.Data;
using NirvanaCommon;
using Version4.Data;

namespace Version4.IO
{
    public class AlleleFrequencyReader : IDisposable
    {
        private readonly Stream               _stream;
        private readonly ExtendedBinaryReader _reader;
        private readonly Block                _block;
        private readonly ZstdContext          _context;

        private readonly ZstdDictionary _dictionary;

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
            int numPositions, string[] alleles, LongHashTable positionAlleles)
        {
            var results = new List<PreloadResult>(numPositions);

            foreach (IndexEntry indexEntry in indexEntries)
            {
                _stream.Position = indexEntry.Offset;

                _block.Read(_reader);
                _block.DecompressDict(_context, _dictionary);

                ReadOnlySpan<byte> byteSpan = _block.UncompressedBytes.AsSpan();

                int numEntries   = SpanBufferBinaryReader.ReadOptInt32(ref byteSpan);
                var lastPosition = 0;

                for (var entryIndex = 0; entryIndex < numEntries; entryIndex++)
                {
                    int position = SpanBufferBinaryReader.ReadOptInt32(ref byteSpan) + lastPosition;

                    if (preloadBitArray.Get(position))
                    {
                        var    variantType = (VariantType) SpanBufferBinaryReader.ReadByte(ref byteSpan);
                        int    alleleIndex = SpanBufferBinaryReader.ReadOptInt32(ref byteSpan);
                        string allele = alleles[alleleIndex];

                        long positionAllele = PositionAllele.Convert(position, allele, variantType);
                        if (positionAlleles.Contains(positionAllele))
                        {
                            string json = SpanBufferBinaryReader.ReadString(ref byteSpan);
                            results.Add(new PreloadResult(position, null, allele, json));
                        }
                        else
                        {
                            SpanBufferBinaryReader.SkipString(ref byteSpan);
                        }
                    }
                    else
                    {
                        SpanBufferBinaryReader.SkipByte(ref byteSpan);
                        SpanBufferBinaryReader.ReadOptInt32(ref byteSpan);
                        SpanBufferBinaryReader.SkipString(ref byteSpan);
                    }

                    lastPosition = position;
                }
            }

            return results;
        }
        
        public string[] GetAlleles(in long fileOffset)
        {
            _stream.Position = fileOffset;
            _block.Read(_reader);
            _block.Decompress(_context);
            
            ReadOnlySpan<byte> byteSpan = _block.UncompressedBytes.AsSpan();

            int numAlleles   = SpanBufferBinaryReader.ReadOptInt32(ref byteSpan);
            var alleles = new string[numAlleles];

            for (var index = 0; index < numAlleles; index++)
            {
                alleles[index] = SpanBufferBinaryReader.ReadString(ref byteSpan);
            }

            return alleles;
        }

        public void Dispose() => _stream.Dispose();
    }
}