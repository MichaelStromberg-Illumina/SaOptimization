using System;
using System.IO;
using Version1.Data;
using Version1.Nirvana;

namespace Version1.IO
{
    public class IndexWriter : IDisposable
    {
        private readonly ExtendedBinaryWriter _writer;
        
        public const byte FileFormatVersion = 1;

        public IndexWriter(Stream stream, IndexHeader header, bool leaveOpen)
        {
            _writer = new ExtendedBinaryWriter(stream, leaveOpen);
            
            WriteHeader(header);
        }

        private void WriteHeader(IndexHeader header)
        {
            _writer.Write(header.Identifier);
            _writer.Write(header.FileFormatVersion);
            _writer.Write((byte)header.GenomeAssembly);
            _writer.Write(header.DataSourceVersion.Name);
            _writer.Write(header.DataSourceVersion.Description);
            _writer.Write(header.DataSourceVersion.Version);
            _writer.Write(header.DataSourceVersion.ReleaseDate.Ticks);
            _writer.Write(header.JsonKey);
            _writer.WriteOpt(header.CompressionDictionary.Length);
            _writer.Write(header.CompressionDictionary);
        }

        public void Write(Data.Index index)
        {
            throw new NotImplementedException();
        }

        public void Dispose() => _writer.Dispose();
    }
}