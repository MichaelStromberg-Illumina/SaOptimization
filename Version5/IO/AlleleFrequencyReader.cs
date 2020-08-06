using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Compression.Data;
using NirvanaCommon;
using Version5.Data;

namespace Version5.IO
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

        public List<PreloadResult> GetAnnotatedVariants(IndexEntry[] indexEntries, LongHashTable positionAlleleSet,
            int numPositions, string[] alleles)
        {
            var results = new List<PreloadResult>(numPositions);

            // int numCommonEntries = 0;
            // int numRareEntries   = 0;

            foreach (IndexEntry indexEntry in indexEntries)
            {
                _stream.Position = indexEntry.Offset;

                _block.Read(_reader);
                _block.DecompressDict(_context, _dictionary);

                ReadOnlySpan<byte> byteSpan = _block.UncompressedBytes.AsSpan();

                int numEntries   = SpanBufferBinaryReader.ReadOptInt32(ref byteSpan);
                var lastPosition = 0;

                // bool isCommon = numEntries > 140;
                
                // Console.WriteLine($"- IndexEntry: common: {isCommon}, offset: {indexEntry.Offset:N0}, # entries: {numEntries:N0}");
                
                for (var entryIndex = 0; entryIndex < numEntries; entryIndex++)
                {
                    int    position    = SpanBufferBinaryReader.ReadOptInt32(ref byteSpan) + lastPosition;
                    var    variantType = (VariantType) SpanBufferBinaryReader.ReadByte(ref byteSpan);
                    int    alleleIndex = SpanBufferBinaryReader.ReadOptInt32(ref byteSpan);
                    string allele      = alleles[alleleIndex];

                    ulong positionAllele = PositionAllele.Convert(position, allele, variantType);
                    if (positionAlleleSet.Contains(positionAllele))
                    {
                        // if (isCommon) numCommonEntries++;
                        // else numRareEntries++;
                        string json = SpanBufferBinaryReader.ReadString(ref byteSpan);
                        results.Add(new PreloadResult(position, null, allele, json));
                    }
                    else
                    {
                        SpanBufferBinaryReader.SkipString(ref byteSpan);
                    }

                    lastPosition = position;
                }
            }
            
            // Console.WriteLine($"- GetAnnotatedVariants: # common entries: {numCommonEntries:N0}, # rare entries: {numRareEntries:N0}");

            return results;
        }

        public string[] GetAlleles(in long fileOffset)
        {
            _stream.Position = fileOffset;
            _block.Read(_reader);
            _block.Decompress(_context);

            ReadOnlySpan<byte> byteSpan   = _block.UncompressedBytes.AsSpan();
            int                numAlleles = SpanBufferBinaryReader.ReadOptInt32(ref byteSpan);

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