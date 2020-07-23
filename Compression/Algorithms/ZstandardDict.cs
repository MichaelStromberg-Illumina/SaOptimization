using System;
using System.Runtime.InteropServices;
using Compression.Data;

namespace Compression.Algorithms
{
    public static class ZstandardDict
    {
        public static int Compress(byte[] source, int srcLength, byte[] destination, int destLength,
            ZstdContext context, ZstdDictionary dict)
        {
            if (destination == null)
                throw new InvalidOperationException("Zstandard: Insufficient memory in destination buffer");

            return (int) SafeNativeMethods.ZSTD_compress_usingCDict(context.ContextPtr, destination, (ulong) destLength,
                source, (ulong) srcLength, dict.DictionaryPtr);
        }

        public static int Decompress(byte[] source, int srcLength, byte[] destination, int destLength,
            ZstdContext context, ZstdDictionary dict)
        {
            if (destination == null)
                throw new InvalidOperationException("Zstandard: Insufficient memory in destination buffer");

            return (int) SafeNativeMethods.ZSTD_decompress_usingDDict(context.ContextPtr, destination,
                (ulong) destLength, source, (ulong) srcLength, dict.DictionaryPtr);
        }

        public static int GetCompressedBufferBounds(int srcSize) =>
            srcSize + (srcSize >> 8) + (srcSize < 128 << 10 ? ((128 << 10) - srcSize) >> 11 : 0);

        private static class SafeNativeMethods
        {
            [DllImport("BlockCompression", CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong ZSTD_compress_usingCDict(IntPtr cctx, byte[] destination, ulong destinationLen, byte[] source, ulong sourceLen, IntPtr cdict);

            [DllImport("BlockCompression", CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong ZSTD_decompress_usingDDict(IntPtr dctx, byte[] destination, ulong destinationLen, byte[] source, ulong sourceLen, IntPtr ddict);
        }
    }
}