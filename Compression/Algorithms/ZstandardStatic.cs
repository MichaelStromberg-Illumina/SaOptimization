using System;
using System.Runtime.InteropServices;
using Compression.Data;

namespace Compression.Algorithms
{
    public static class ZstandardStatic
    {
        public static int Compress(byte[] source, int srcLength, byte[] destination, int destLength,
            ZstdContext context, int compressionLevel = 17)
        {
            if (destination == null)
                throw new InvalidOperationException("Zstandard: Insufficient memory in destination buffer");

            return (int) SafeNativeMethods.ZSTD_compressCCtx(context.ContextPtr, destination, (ulong) destLength,
                source, (ulong) srcLength, compressionLevel);
        }

        public static int Decompress(byte[] source, int srcLength, byte[] destination, int destLength,
            ZstdContext context)
        {
            if (destination == null)
                throw new InvalidOperationException("Zstandard: Insufficient memory in destination buffer");
            
            return (int) SafeNativeMethods.ZSTD_decompressDCtx(context.ContextPtr, destination, (ulong) destLength,
                source, (ulong) srcLength);
        }

        public static int GetCompressedBufferBounds(int srcSize) =>
            srcSize + (srcSize >> 8) + (srcSize < 128 << 10 ? ((128 << 10) - srcSize) >> 11 : 0);

        private static class SafeNativeMethods
        {
            [DllImport("BlockCompression", CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong ZSTD_compressCCtx(IntPtr cctx, byte[] destination, ulong destinationLen,
                byte[] source, ulong sourceLen, int compressionLevel);

            [DllImport("BlockCompression", CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong ZSTD_decompressDCtx(IntPtr dctx, byte[] destination, ulong destinationLen,
                byte[] source, ulong sourceLen);
        }
    }
}