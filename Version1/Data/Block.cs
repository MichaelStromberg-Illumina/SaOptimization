using System.Collections.Generic;
using System.IO;
using Version1.Nirvana;
using Version1.Utilities;

namespace Version1.Data
{
    public sealed class Block
    {
        public readonly  int    LastPosition;
        private readonly int    _numEntries;
        private readonly byte[] _compressedBytes;
        private readonly int    _numCompressedBytes;

        public long FileOffset { get; private set; }

        public Block(int lastPosition, int numEntries, byte[] compressedBytes, int numCompressedBytes)
        {
            LastPosition        = lastPosition;
            _numEntries         = numEntries;
            _compressedBytes    = compressedBytes;
            _numCompressedBytes = numCompressedBytes;
        }

        // public Block(ICompressionAlgorithm compressionAlgorithm, List<TsvEntry> entries)
        // {
        //     LastPosition = entries[^1].Position;
        //     _numEntries  = entries.Count;
        //
        //     byte[] dataBytes = GetBytes(entries);
        //     int numDataBytes = dataBytes.Length;
        //
        //     int compressedBufferSize = compressionAlgorithm.GetCompressedBufferBounds(numDataBytes);
        //     _compressedBytes = new byte[compressedBufferSize];
        //
        //     _numCompressedBytes = compressionAlgorithm.Compress(dataBytes, numDataBytes, 
        //         _compressedBytes, compressedBufferSize);
        // }

        private static byte[] GetBytes(List<TsvEntry> entries)
        {
            byte[] bytes;

            var lastPosition = 0;

            using (var stream = new MemoryStream())
            using (var writer = new ExtendedBinaryWriter(stream))
            {
                foreach (TsvEntry entry in entries)
                {
                    VariantType variantType = VariantTypeUtils.GetVariantType(entry.RefAllele, entry.AltAllele);
                    VariantTypeUtils.CheckVariantType(variantType);

                    string allele        = variantType == VariantType.deletion ? entry.RefAllele : entry.AltAllele;
                    int    deltaPosition = entry.Position - lastPosition;

                    writer.Write((byte) variantType);
                    writer.WriteOpt(deltaPosition);
                    writer.Write(allele);
                    writer.Write(entry.Json);

                    lastPosition = entry.Position;
                }

                bytes = stream.ToArray();
            }

            return bytes;
        }

        public void Write(ExtendedBinaryWriter writer)
        {
            FileOffset = writer.BaseStream.Position;
            writer.WriteOpt(_numEntries);
            writer.Write(_compressedBytes, 0, _numCompressedBytes);
        }
    }
}