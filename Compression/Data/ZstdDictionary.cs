using System;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace Compression.Data
{
    public class ZstdDictionary : IDisposable
    {
        private readonly  CompressionMode _mode;
        internal readonly IntPtr          DictionaryPtr;

        public ZstdDictionary(CompressionMode mode, byte[] dictionaryBytes, int compressionLevel)
        {
            _mode = mode;
            DictionaryPtr = _mode == CompressionMode.Compress
                ? SafeNativeMethods.ZSTD_createCDict(dictionaryBytes, (ulong) dictionaryBytes.Length, compressionLevel)
                : SafeNativeMethods.ZSTD_createDDict(dictionaryBytes, (ulong) dictionaryBytes.Length);
        }

        public void Dispose()
        {
            if (_mode == CompressionMode.Compress) SafeNativeMethods.ZSTD_freeCDict(DictionaryPtr);
            else SafeNativeMethods.ZSTD_freeDDict(DictionaryPtr);
        }

        private static class SafeNativeMethods
        {
            #region Compression Dictionary

            [DllImport("BlockCompression", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr ZSTD_createCDict(byte[] dictBuffer, ulong dictSize, int compressionLevel);

            [DllImport("BlockCompression", CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong ZSTD_freeCDict(IntPtr cdict);

            #endregion

            #region Decompression Dictionary

            [DllImport("BlockCompression", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr ZSTD_createDDict(byte[] dictBuffer, ulong dictSize);

            [DllImport("BlockCompression", CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong ZSTD_freeDDict(IntPtr ddict);

            #endregion
        }
    }
}