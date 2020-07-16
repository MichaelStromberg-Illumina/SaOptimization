using System;
using System.Collections.Generic;
using System.IO;
using Compression.Algorithms;
using Version1.Data;
using Version1.Nirvana;

namespace Version1.IO
{
    public class AlleleFrequencyWriter : IDisposable
    {
        private readonly ExtendedBinaryWriter _writer;
        private readonly int _numRefSeqs;

        private readonly IndexWriter _indexWriter;
        private readonly Data.Index _index;

        private readonly ICompressionAlgorithm _zstd;

        public const byte FileFormatVersion = 1;

        public AlleleFrequencyWriter(Stream            stream, Stream indexStream, GenomeAssembly genomeAssembly,
                                     DataSourceVersion dataSourceVersion, string jsonKey, byte[] dictionaryBytes,
                                     int               numRefSeqs, bool leaveOpen = false)
        {
            _numRefSeqs = numRefSeqs;
            _writer     = new ExtendedBinaryWriter(stream, leaveOpen);

            var header = new Header(SaConstants.AlleleFrequencyIdentifier, FileFormatVersion);
            WriteHeader(header);

            var indexHeader = new IndexHeader(SaConstants.IndexIdentifier, IndexWriter.FileFormatVersion,
                genomeAssembly, dataSourceVersion, jsonKey, dictionaryBytes);
            _indexWriter = new IndexWriter(indexStream, indexHeader, leaveOpen);
            
            _zstd = new ZstandardDict(17, dictionaryBytes);
        }

        private void WriteHeader(Header header)
        {
            _writer.Write(header.Identifier);
            _writer.Write(header.FileFormatVersion);
        }

        public void WriteBlocks(Chromosome chromosome, List<Block> blocks)
        {
            _writer.WriteOpt(blocks.Count);
            foreach (Block block in blocks) block.Write(_writer);
        }
        
        // public void WriteChromosome(Chromosome chromosome, List<Block> commonBlocks, List<Block> rareBlocks)
        // {
        //
        //     
        //
        //     CreateCommonBlocks(commonEntries);
        //     Console.WriteLine($"- memory usage: {Process.GetCurrentProcess().WorkingSet64:N0} bytes");
        //     
        //     CreateRareBlocks(rareEntries);
        //     Console.WriteLine($"- memory usage: {Process.GetCurrentProcess().WorkingSet64:N0} bytes");
        // }

        // private void CreateRareBlocks(TsvEntry[] rareEntries)
        // {
        //     Console.Write("- creating rare blocks... ");
        //     List<Block> rareBlocks = BlockUtilities.Create(rareEntries, SaConstants.MaxRareEntries, _zstd);
        //     Console.WriteLine($"{rareBlocks.Count} blocks created.");
        //     
        //     Console.Write("- writing rare blocks... ");
        //     _writer.WriteOpt(rareBlocks.Count);
        //     foreach (Block block in rareBlocks) block.Write(_writer);
        //     Console.WriteLine("finished.");
        // }
        //
        // private void CreateCommonBlocks(TsvEntry[] commonEntries)
        // {
        //     Console.Write("- creating common blocks... ");
        //     List<Block> commonBlocks = BlockUtilities.Create(commonEntries, SaConstants.MaxCommonEntries, _zstd);
        //     Console.WriteLine($"{commonBlocks.Count} blocks created.");
        //     
        //     Console.Write("- writing common blocks... ");
        //     _writer.WriteOpt(commonBlocks.Count);
        //     foreach (Block block in commonBlocks) block.Write(_writer);
        //     Console.WriteLine("finished.");
        // }

        public void WriteIndex(Stream stream, bool leaveOpen)
        {
            using (var writer = new IndexWriter(stream, _index.Header, leaveOpen))
            {
                writer.Write(_index);
            }
        }

        public void Dispose() => _writer.Dispose();
    }
}