using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Compression.Data;
using NirvanaCommon;
using VariantGrouping;
using Version1.Data;

namespace Version1.IO
{
    public class AlleleFrequencyReader : IDisposable
    {
        private readonly Stream               _stream;
        private readonly ExtendedBinaryReader _reader;

        private readonly ZstdDictionary _dictionary;
        public ZstdDictionary Dictionary => _dictionary;

        public AlleleFrequencyReader(Stream stream, bool leaveOpen = false)
        {
            _stream = stream;
            _reader = new ExtendedBinaryReader(stream, leaveOpen);

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

        public List<AnnotatedVariant> GetAnnotatedVariants(BlockRange[] blockRanges, BitArray preloadBitArray,
            List<PreloadVariant> variants, ZstdContext context, ZstdDictionary dictionary)
        {
            var annotations = new List<AnnotatedVariant>(variants.Count);
            var block       = new Block(null, 0, 0);
            int numVariants = 0;

            foreach (BlockRange blockRange in blockRanges)
            {
                _stream.Position = blockRange.FileOffset;

                for (var numBlocks = 0; numBlocks < blockRange.NumBlocks; numBlocks++)
                {
                    block.Read(_reader);
                    block.Decompress(context, dictionary);

                    var reader       = new BufferBinaryReader(block.UncompressedBytes);
                    int numEntries   = reader.ReadOptInt32();
                    int lastPosition = 0;

                    for (int entryIndex = 0; entryIndex < numEntries; entryIndex++)
                    {
                        int position = reader.ReadOptInt32() + lastPosition;

                        if (preloadBitArray.Get(position))
                        {
                            var variantType = (VariantType) reader.ReadByte();
                            string allele = reader.ReadString();
                            string json   = reader.ReadString();
                            annotations.Add(new AnnotatedVariant(position, variantType, allele, json));
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
            }

            return annotations;
        }

        public void Dispose() => _stream.Dispose();
    }
}