﻿using System;
using System.IO;
using NirvanaCommon;
using Version2.Data;

namespace Version2.IO
{
    public class IndexReader : IDisposable
    {
        private readonly Stream _stream;
        private readonly ExtendedBinaryReader _reader;
        
        private readonly long[] _chromosomeOffsets;
        
        private ushort _currentRefIndex = UInt16.MaxValue;
        private ChromosomeIndex _currentIndex;

        public IndexReader(Stream stream, bool leaveOpen = false)
        {
            _stream    = stream;
            _reader    = new ExtendedBinaryReader(_stream, leaveOpen);

            IndexHeader header = IndexHeader.Read(_reader);
            CheckHeader(header);

            _chromosomeOffsets = ReadChromosomeOffsets(header.NumRefSeqs);
        }

        private long[] ReadChromosomeOffsets(int numRefSeqs)
        {
            var chromosomeOffsets = new long[numRefSeqs];
            for (var i = 0; i < numRefSeqs; i++) chromosomeOffsets[i] = _reader.ReadInt64();
            return chromosomeOffsets;
        }

        private static void CheckHeader(IndexHeader header)
        {
            if (header.Identifier != SaConstants.IndexIdentifier)
                throw new InvalidDataException(
                    $"Invalid index file. Identifier did not match: '{SaConstants.IndexIdentifier}");
            
            if (header.FileFormatVersion != IndexWriter.FileFormatVersion)
                throw new InvalidDataException(
                    $"Unsupported file format version (supported: {IndexWriter.FileFormatVersion} vs file: {header.FileFormatVersion}");
        }

        public void Dispose() => _reader.Dispose();

        public ChromosomeIndex Load(Chromosome chromosome)
        {
            ushort refIndex = chromosome.Index;
            if (refIndex == _currentRefIndex) return _currentIndex;

            long fileOffset = _chromosomeOffsets[refIndex];
            _stream.Position = fileOffset;

            var index = ChromosomeIndex.Read(_reader);
            _currentIndex    = index;
            _currentRefIndex = refIndex;

            return index;
        }
    }
}