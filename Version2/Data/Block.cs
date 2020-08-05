using Compression.Algorithms;
using Compression.Data;
using NirvanaCommon;

namespace Version2.Data
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
                var newSize = (int)(NumCompressedBytes * PercentAdditionalBytes);
                CompressedBytes = new byte[newSize];
            }

            reader.ReadOptBytes(CompressedBytes, NumCompressedBytes);
        }

        public void Decompress(ZstdContext context, ZstdDictionary dictionary)
        {
            if (UncompressedBytes == null || NumUncompressedBytes > UncompressedBytes.Length)
            {
                var newSize = (int)(NumUncompressedBytes * PercentAdditionalBytes);
                UncompressedBytes = new byte[newSize];
            }

            ZstandardDict.Decompress(CompressedBytes, NumCompressedBytes, UncompressedBytes, NumUncompressedBytes,
                context, dictionary);
        }
    }
}