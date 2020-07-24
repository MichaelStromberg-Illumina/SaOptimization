﻿using System;
using System.IO;
using Compression.Data;
using NirvanaCommon;
using Version2.Data;

namespace Version2.IO
{
    public class IndexWriter : IDisposable
    {
        private readonly Stream               _stream;
        private readonly ExtendedBinaryWriter _writer;

        private readonly long[] _chromosomeOffsets;
        private readonly long   _chromosomeIndexOffset;

        public const byte FileFormatVersion = 1;

        public IndexWriter(Stream stream, int numRefSeqs, bool leaveOpen = false)
        {
            _stream            = stream;
            _writer            = new ExtendedBinaryWriter(stream, leaveOpen);
            _chromosomeOffsets = new long[numRefSeqs];

            var indexHeader = new IndexHeader(SaConstants.IndexIdentifier, FileFormatVersion, numRefSeqs);
            indexHeader.Write(_writer);

            _chromosomeIndexOffset = stream.Position;
            WriteChromosomeOffsets();
        }

        private void WriteChromosomeOffsets()
        {
            foreach (long offset in _chromosomeOffsets) _writer.Write(offset);
        }

        public void Write(ChromosomeIndex[] chromsomeIndices, ZstdContext context, ZstdDictionary dict)
        {
            if (chromsomeIndices.Length != _chromosomeOffsets.Length)
                throw new InvalidDataException(
                    $"Found a mismatch in the number of reference sequences ({chromsomeIndices.Length} vs {_chromosomeOffsets.Length})");

            var refIndex = 0;
            foreach (ChromosomeIndex chromosomeIndex in chromsomeIndices)
            {
                _chromosomeOffsets[refIndex++] = _stream.Position;
                chromosomeIndex.Write(_writer, context, dict);
            }
            
            // write the chromosome offsets
            _stream.Position = _chromosomeIndexOffset;
            WriteChromosomeOffsets();
        }

        public void Dispose() => _writer.Dispose();
    }
}