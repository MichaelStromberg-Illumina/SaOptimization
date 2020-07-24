using System;
using System.Runtime.InteropServices;

namespace Compression.Algorithms
{
    public sealed class Zstandard : ICompressionAlgorithm
    {
        public int Decompress(byte[] source, int srcLength, byte[] destination, int destLength)
        {
            if (destination == null)
            {
                throw new InvalidOperationException("Zstandard: Insufficient memory in destination buffer");
            }

            return (int)SafeNativeMethods.ZSTD_decompress(destination, (ulong)destLength, source, (ulong)srcLength);
        }

        // empirically derived via polynomial regression with additional padding added
        public int GetCompressedBufferBounds(int srcLength) => srcLength + 32;

        private static class SafeNativeMethods
        {
            [DllImport("BlockCompression", CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong ZSTD_decompress(byte[] destination, ulong destinationLen, byte[] source, ulong sourceLen);
        }
    }
}
