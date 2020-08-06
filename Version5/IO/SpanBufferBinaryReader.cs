using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace Version5.IO
{
    public static class SpanBufferBinaryReader
    {
        private const int MostSignificantBit = 128;
        private const int VlqBitShift        = 7;

        private static readonly Encoding Encoding = Encoding.UTF8;
        private static readonly Decoder  Decoder  = Encoding.UTF8.GetDecoder();
        
        public static int ReadOptInt32(ref ReadOnlySpan<byte> byteSpan)
        {
            var count = 0;
            var shift = 0;
            var index = 0;

            while (shift != 35)
            {
                byte b = byteSpan[index++];
                count |= (b & sbyte.MaxValue) << shift;
                shift += VlqBitShift;

                if ((b & MostSignificantBit) == 0)
                {
                    byteSpan = byteSpan.Slice(index);
                    return count;
                }
            }

            throw new FormatException("Unable to read the 7-bit encoded integer");
        }

        public static long ReadOptInt64(ref ReadOnlySpan<byte> byteSpan)
        {
            long count = 0;
            var  shift = 0;
            var  index = 0;

            while (shift != 70)
            {
                byte b = byteSpan[index++];
                count |= (long) (b & sbyte.MaxValue) << shift;
                shift += VlqBitShift;

                if ((b & MostSignificantBit) == 0)
                {
                    byteSpan = byteSpan.Slice(index);
                    return count;
                }
            }

            throw new FormatException("Unable to read the 7-bit encoded long");
        }

        public static ulong ReadOptUInt64(ref ReadOnlySpan<byte> byteSpan)
        {
            ulong count = 0;
            var   shift = 0;
            var   index = 0;

            while (shift != 70)
            {
                byte b = byteSpan[index++];
                count |= (ulong) (b & sbyte.MaxValue) << shift;
                shift += VlqBitShift;

                if ((b & MostSignificantBit) == 0)
                {
                    byteSpan = byteSpan.Slice(index);
                    return count;
                }
            }

            throw new FormatException("Unable to read the 7-bit encoded ulong");
        }
        
        public static string ReadString(ref ReadOnlySpan<byte> byteSpan)
        {
            int numBytes = ReadOptInt32(ref byteSpan);
            if (numBytes == 0) return string.Empty;

            int        maxBufferSize = Encoding.GetMaxCharCount(numBytes);
            char[]     charBuffer    = ArrayPool<char>.Shared.Rent(maxBufferSize);
            Span<char> charSpan      = charBuffer.AsSpan();

            int numChars = Decoder.GetChars(byteSpan.Slice(0, numBytes), charSpan, true);
            charSpan = charSpan.Slice(0, numChars);
            byteSpan = byteSpan.Slice(numBytes);
            ArrayPool<char>.Shared.Return(charBuffer);
            
            return new string(charSpan);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SkipString(ref ReadOnlySpan<byte> byteSpan)
        {
            int numBytes = ReadOptInt32(ref byteSpan);
            if (numBytes == 0) return;
            byteSpan = byteSpan.Slice(numBytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadByte(ref ReadOnlySpan<byte> byteSpan)
        {
            byte value = byteSpan[0];
            byteSpan = byteSpan.Slice(1);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<byte> ReadBytes(ref ReadOnlySpan<byte> byteSpan, int numBytes)
        {
            ReadOnlySpan<byte> value = byteSpan.Slice(0, numBytes);
            byteSpan = byteSpan.Slice(numBytes);
            return value;
        }
    }
}