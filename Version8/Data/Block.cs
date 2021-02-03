using System.Buffers;
using Compression.Algorithms;
using Compression.Data;
using NirvanaCommon;

namespace Version8.Data
{
    public class Block
    {
        protected byte[] CompressedBytes;
        protected int    NumCompressedBytes;

        public    byte[] UncompressedBytes;
        protected int    NumUncompressedBytes;

        private const double PercentAdditionalBytes = 1.02;

        public Block(byte[] compressedBytes, int numCompressedBytes, int numUncompressedBytes)
        {
            CompressedBytes      = compressedBytes;
            NumCompressedBytes   = numCompressedBytes;
            NumUncompressedBytes = numUncompressedBytes;
        }

        public void Read(ExtendedBinaryReader reader)
        {
            NumCompressedBytes   = reader.ReadOptInt32();
            NumUncompressedBytes = reader.ReadOptInt32() + NumCompressedBytes;

            if (CompressedBytes == null || NumCompressedBytes > CompressedBytes.Length)
            {
                if (CompressedBytes != null) ArrayPool<byte>.Shared.Return(CompressedBytes);
                var newSize = (int) (NumCompressedBytes * PercentAdditionalBytes);
                CompressedBytes = ArrayPool<byte>.Shared.Rent(newSize);
            }

            reader.ReadOptBytes(CompressedBytes, NumCompressedBytes);
        }

        public void DecompressDict(ZstdContext context, ZstdDictionary dictionary)
        {
            if (UncompressedBytes == null || NumUncompressedBytes > UncompressedBytes.Length) Resize();
            ZstandardDict.Decompress(CompressedBytes, NumCompressedBytes, UncompressedBytes, NumUncompressedBytes,
                context, dictionary);
        }
        
        public void Decompress(ZstdContext context)
        {
            if (UncompressedBytes == null || NumUncompressedBytes > UncompressedBytes.Length) Resize();
            ZstandardStatic.Decompress(CompressedBytes, NumCompressedBytes, UncompressedBytes, NumUncompressedBytes,
                context);
        }

        private void Resize()
        {
            if (UncompressedBytes != null) ArrayPool<byte>.Shared.Return(UncompressedBytes);
            var newSize = (int) (NumUncompressedBytes * PercentAdditionalBytes);
            UncompressedBytes = ArrayPool<byte>.Shared.Rent(newSize);
        }
    }
}