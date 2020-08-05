using System;
using System.Buffers;
using System.Text;

namespace Version4.IO
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

        public static void SkipString(ref ReadOnlySpan<byte> byteSpan)
        {
            int numBytes = ReadOptInt32(ref byteSpan);
            if (numBytes == 0) return;
            byteSpan = byteSpan.Slice(numBytes);
        }

        public static byte ReadByte(ref ReadOnlySpan<byte> byteSpan)
        {
            byte value = byteSpan[0];
            byteSpan = byteSpan.Slice(1);
            return value;
        }
        
        public static void SkipByte(ref ReadOnlySpan<byte> byteSpan) => byteSpan = byteSpan.Slice(1);
    }
}