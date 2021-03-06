﻿using System;
using System.IO;
using NirvanaCommon;
using Version7.Data;

namespace Version7.IO
{
    public sealed class AlleleFrequencyWriter : IDisposable
    {
        private readonly Stream _stream;
        private readonly ExtendedBinaryWriter _writer;
        private readonly IndexBuilder _indexBuilder;
        
        public const byte FileFormatVersion = 1;

        private int _numBlocks;
        private long _sectionFileOffset;
        private bool _useCommon;

        public AlleleFrequencyWriter(Stream stream, GenomeAssembly genomeAssembly, DataSourceVersion dataSourceVersion,
            string jsonKey, int numRefSeqs, bool leaveOpen = false)
        {
            _stream     = stream;
            _writer     = new ExtendedBinaryWriter(stream, leaveOpen);

            var header = new Header(SaConstants.AlleleFrequencyIdentifier, FileFormatVersion, numRefSeqs,
                genomeAssembly, dataSourceVersion, jsonKey);
            header.Write(_writer);
            
            _indexBuilder = new IndexBuilder(numRefSeqs);
        }

        public void EndChromosome(Chromosome chromosome, Xor8 xorFilter, ulong[] commonPositionAlleles)
        {
            ushort refIndex = chromosome.Index;
            _indexBuilder.FinalizeChromosome(refIndex, xorFilter, commonPositionAlleles);
        }

        private void InitializeSection(bool addingCommonBlocks)
        {
            _useCommon         = addingCommonBlocks;
            _sectionFileOffset = _stream.Position;
            _numBlocks         = 0;
            _writer.Write(_numBlocks);
        }

        private void UpdateBlockCount()
        {
            long currentOffset = _stream.Position;
            _stream.Position = _sectionFileOffset;
            _writer.Write(_numBlocks);
            _stream.Position = currentOffset;
        }

        public ChromosomeIndex[] ChromosomeIndices => _indexBuilder.ChromosomeIndices;
        
        public void  StartCommon() => InitializeSection(true);
        public void  EndCommon()   => UpdateBlockCount();
        public void  StartRare()   => InitializeSection(false);
        public void  EndRare()     => UpdateBlockCount();

        public void WriteAlleles(WriteBlock block)
        {
            // Console.WriteLine($"  - allele index file offset: {_stream.Position:N0}");
            _indexBuilder.SetAlleleIndexOffset(_stream.Position);
            block.Write(_writer);
        }

        public void WriteBlock(WriteBlock block)
        {
            _indexBuilder.AddBlock(block.LastPosition, _stream.Position, _useCommon);
            block.Write(_writer);
        }

        public void Dispose() => _writer.Dispose();
    }
}