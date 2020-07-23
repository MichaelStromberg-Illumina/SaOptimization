using System;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace Compression.Data
{
    public class ZstdContext : IDisposable
    {
        private readonly  CompressionMode _mode;
        internal readonly IntPtr          ContextPtr;

        public ZstdContext(CompressionMode mode)
        {
            _mode = mode;
            ContextPtr = _mode == CompressionMode.Compress
                ? SafeNativeMethods.ZSTD_createCCtx()
                : SafeNativeMethods.ZSTD_createDCtx();
        }
        
        public void Dispose()
        {
            if (_mode == CompressionMode.Compress) SafeNativeMethods.ZSTD_freeCCtx(ContextPtr);
            else SafeNativeMethods.ZSTD_freeDCtx(ContextPtr);
        }
        
        private static class SafeNativeMethods
        {
            #region Compression Context

            [DllImport("BlockCompression", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr ZSTD_createCCtx();

            [DllImport("BlockCompression", CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong ZSTD_freeCCtx(IntPtr cctx);

            #endregion

            #region Decompression Context

            [DllImport("BlockCompression", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr ZSTD_createDCtx();

            [DllImport("BlockCompression", CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong ZSTD_freeDCtx(IntPtr dctx);

            #endregion
        }
    }
}