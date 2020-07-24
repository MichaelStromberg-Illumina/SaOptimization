using System;
using System.Collections.Generic;
using System.IO;
using Compression.Algorithms;
using NirvanaCommon;

namespace PreloadBaseline.Nirvana
{
    public sealed class NsaReader : IDisposable
    {
        private readonly Stream _stream;
        private readonly ExtendedBinaryReader _reader;
        public GenomeAssembly Assembly { get; }
        private readonly NsaIndex _index;
        public DataSourceVersion Version { get; }

        private readonly NsaBlock _block;

        public string JsonKey { get; }
        public bool MatchByAllele { get; }
        public bool IsArray { get; }
        public bool IsPositional { get; }
        private readonly List<AnnotationItem> _annotations;
        private readonly int _blockSize;

        private readonly ExtendedBinaryReader _annotationReader;
        private readonly MemoryStream _annotationStream;
        private readonly byte[] _annotationBuffer;


        public NsaReader(Stream dataStream, Stream indexStream, int blockSize = SaCommon.DefaultBlockSize)
        {
            _stream    = dataStream;
            _blockSize = blockSize;
            _reader    = new ExtendedBinaryReader(_stream);
            _block     = new NsaBlock(new Zstandard(), blockSize);

            _index        = new NsaIndex(indexStream);
            Assembly      = _index.Assembly;
            Version       = _index.Version;
            JsonKey       = _index.JsonKey;
            MatchByAllele = _index.MatchByAllele;
            IsArray       = _index.IsArray;
            IsPositional  = _index.IsPositional;

            if (_index.SchemaVersion != SaCommon.SchemaVersion)
                throw new InvalidDataException(
                    $"SA schema version mismatch. Expected {SaCommon.SchemaVersion}, observed {_index.SchemaVersion} for {JsonKey}");

            _annotations      = new List<AnnotationItem>(64 * 1024);
            _annotationBuffer = new byte[1024 * 1024];
            _annotationStream = new MemoryStream(_annotationBuffer);
            _annotationReader = new ExtendedBinaryReader(_annotationStream);
        }

        public List<PreloadResult> PreLoad(Chromosome chrom, List<int> positions)
        {
            if (positions == null || positions.Count == 0) return null;

            _annotations.Clear();
            for (var i = 0; i < positions.Count;)
            {
                int  position     = positions[i];
                long fileLocation = _index.GetFileLocation(chrom.Index, position);
                if (fileLocation == -1)
                {
                    i++;
                    continue;
                }

                //only reconnect if necessary
                if (_reader.BaseStream.Position != fileLocation)
                    _reader.BaseStream.Position = fileLocation;
                _block.Read(_reader);
                var newIndex = _block.AddAnnotations(positions, i, _annotations);
                if (newIndex == i) i++; //no positions were found in this block
                else i = newIndex;
            }

            return GetPreloadedVariants(_annotations);
        }

        private List<PreloadResult> GetPreloadedVariants(List<AnnotationItem> annotations)
        {
            var results = new List<PreloadResult>(annotations.Count);

            foreach (AnnotationItem annotationItem in annotations)
            {
                Array.Copy(annotationItem.Data, _annotationBuffer, annotationItem.Data.Length);
                _annotationStream.Position = 0;

                int numAlleles = _annotationReader.ReadOptInt32();
                for (var alleleIndex = 0; alleleIndex < numAlleles; alleleIndex++)
                {
                    string refAllele = _annotationReader.ReadAsciiString();
                    string altAllele = _annotationReader.ReadAsciiString();
                    string json      = _annotationReader.ReadString();

                    results.Add(new PreloadResult(annotationItem.Position, refAllele, altAllele, json));
                }
            }

            return results;
        }

        public void Dispose()
        {
            _stream?.Dispose();
            _block?.Dispose();
            _annotationStream?.Dispose();
            _annotationReader?.Dispose();
        }
    }
}