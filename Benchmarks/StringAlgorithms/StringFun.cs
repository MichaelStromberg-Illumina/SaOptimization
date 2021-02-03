using System.Numerics;
using System.Runtime.CompilerServices;

namespace Benchmarks.StringAlgorithms
{
    public class StringFun
    {
        /// <summary>
        /// 'Widen' each byte in 'bytes' to 16-bits with no consideration for
        /// character mapping or encoding.
        /// </summary>
        public static unsafe string ByteArrayToString(byte[] bytes)
        {
            // note: possible zeroing penalty; consider buffer pooling or 
            // other ways to allocate target?
            var s = new string('\0', bytes.Length);

            if (s.Length > 0)
                fixed (char* dst = s)
                fixed (byte* src = bytes)
                    widen_bytes_simd(dst, src, s.Length);
            return s;
        }

        private static unsafe void widen_bytes_simd(char* dst, byte* src, int c)
        {
            for (; c > 0 && ((long)dst & 0xF) != 0; c--)
                *dst++ = (char)*src++;

            for (; (c -= 0x10) >= 0; src += 0x10, dst += 0x10)
                Vector.Widen(Unsafe.AsRef<Vector<byte>>(src),
                    out Unsafe.AsRef<Vector<ushort>>(dst + 0),
                    out Unsafe.AsRef<Vector<ushort>>(dst + 8));

            for (c += 0x10; c > 0; c--)
                *dst++ = (char)*src++;
        }
    }
}