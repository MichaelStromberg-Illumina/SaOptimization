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
            // Console.WriteLine($"# compressed bytes: {NumCompressedBytes:N0}, # uncompressed bytes: {NumUncompressedBytes:N0}");

            if (CompressedBytes == null || NumCompressedBytes > CompressedBytes.Length)
            {
                int newSize = (int)(NumCompressedBytes * PercentAdditionalBytes);
                // Console.WriteLine($"Read:       reallocate to {newSize:N0}");
                CompressedBytes = new byte[newSize];
            }

            reader.ReadOptBytes(CompressedBytes, NumCompressedBytes);
        }

        public void Decompress(ZstdContext context, ZstdDictionary dictionary)
        {
            if (UncompressedBytes == null || NumUncompressedBytes > UncompressedBytes.Length)
            {
                int newSize = (int)(NumUncompressedBytes * PercentAdditionalBytes);
                // Console.WriteLine($"Decompress: reallocate to {newSize:N0}");
                UncompressedBytes = new byte[newSize];
            }

            ZstandardDict.Decompress(CompressedBytes, NumCompressedBytes, UncompressedBytes, NumUncompressedBytes,
                context, dictionary);
        }
    }
}