using System;
using System.IO;
using Compression.Data;
using NirvanaCommon;
using Version5.Data;

namespace Version5.IO
{
    public class IndexReader : IDisposable
    {
        private readonly Stream _stream;
        private readonly Block _block;
        private readonly ZstdContext _context;
        private readonly ExtendedBinaryReader _reader;
        
        private readonly long[] _chromosomeOffsets;
        
        private ushort _currentRefIndex = ushort.MaxValue;
        private ChromosomeIndex _currentIndex;

        public IndexReader(Stream stream, Block block, ZstdContext context, bool leaveOpen = false)
        {
            _stream  = stream;
            _reader  = new ExtendedBinaryReader(_stream, leaveOpen);
            _block   = block;
            _context = context;

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

            ChromosomeIndex index = ChromosomeIndexReader.Read(_reader, _block, _context);
            _currentIndex    = index;
            _currentRefIndex = refIndex;

            return index;
        }
    }
}