using System;
using System.Collections.Generic;
using System.IO;

namespace PreloadBaseline.Nirvana
{
    public sealed class NsaBlock : IDisposable
    {
        private readonly ICompressionAlgorithm _compressionAlgorithm;
        private readonly byte[] _compressedBlock;
        private readonly byte[] _uncompressedBlock;
        private int _compressedLength;
        private int _uncompressedLength;
        private int _firstPosition;
        private int _lastPosition;
        private int _count;

        private readonly ExtendedBinaryReader _blockReader;
        private readonly MemoryStream _blockStream;


        public NsaBlock(ICompressionAlgorithm compressionAlgorithm, int size)
        {
            _compressionAlgorithm = compressionAlgorithm;
            _uncompressedBlock    = new byte[size];
            _blockStream          = new MemoryStream(_uncompressedBlock);
            _blockReader          = new ExtendedBinaryReader(_blockStream);

            int compressedBlockSize = compressionAlgorithm.GetCompressedBufferBounds(size);
            _compressedBlock = new byte[compressedBlockSize];
        }

        public void Read(ExtendedBinaryReader reader)
        {
            _compressedLength = reader.ReadOptInt32();
            _firstPosition    = reader.ReadOptInt32();
            //_lastPosition   = reader.ReadOptInt32();
            _count = reader.ReadOptInt32();
            reader.Read(_compressedBlock, 0, _compressedLength);

            _uncompressedLength = _compressionAlgorithm.Decompress(_compressedBlock, _compressedLength,
                _uncompressedBlock, _uncompressedBlock.Length);

            _blockStream.Position = 0;
        }

        public int AddAnnotations(List<int> vcfPositions, int j, List<AnnotationItem> annotationItems)
        {
            if (_uncompressedLength == 0) return j;

            _blockStream.Position = 0;
            var position = _firstPosition;

            var i      = 0;
            var length = _blockReader.ReadOptInt32();
            position += _blockReader.ReadOptInt32();

            while (i < _count && j < vcfPositions.Count)
            {
                if (position < vcfPositions[j])
                {
                    _blockStream.Position += length;
                    //this position is not needed, move to next
                    length   =  _blockReader.ReadOptInt32();
                    position += _blockReader.ReadOptInt32();
                    i++;
                    continue;
                }

                if (vcfPositions[j] < position)
                {
                    //go to next position from vcf
                    j++;
                    continue;
                }

                //positions have matched
                var data = _blockReader.ReadBytes(length);

                annotationItems.Add(new AnnotationItem(position, data));

                j++;
                i++;
                length   =  _blockReader.ReadOptInt32();
                position += _blockReader.ReadOptInt32();
            }

            return j;
        }

        public void Dispose()
        {
            _blockReader?.Dispose();
            _blockStream?.Dispose();
        }
    }
}