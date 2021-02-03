using System;

namespace Benchmarks.VlqAlgorithms
{
    public static class SpanReader
    {
        private const int MostSignificantBit = 128;
        private const int VlqBitShift        = 7;

        private const byte VlqBitShift2        = 7;
        private const byte MostSignificantBit2 = 128;
        
        public static int Version1(ref ReadOnlySpan<byte> byteSpan)
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
        
        public static int Version2(ref ReadOnlySpan<byte> byteSpan)
        {
            uint result = 0;
            var  index  = 0;
            byte byteReadJustNow;

            const int MaxBytesWithoutOverflow = 4;
            for (var shift = 0; shift < MaxBytesWithoutOverflow * 7; shift += 7)
            {
                byteReadJustNow =  byteSpan[index++];
                result          |= (byteReadJustNow & 0x7Fu) << shift;

                if (byteReadJustNow <= 0x7Fu)
                {
                    byteSpan = byteSpan.Slice(index);
                    return (int) result;
                }
            }

            byteReadJustNow = byteSpan[index++];
            if (byteReadJustNow > 0b_1111u) throw new FormatException("Unable to read the 7-bit encoded integer");

            byteSpan = byteSpan.Slice(index);
            result |= (uint) byteReadJustNow << (MaxBytesWithoutOverflow * 7);
            return (int) result;
        }

        public static int Version3(ref ReadOnlySpan<byte> byteSpan)
        {
            var  count = 0;
            byte shift = 0;
            byte index = 0;

            while (shift != 35)
            {
                byte b = byteSpan[index++];
                count |= (b & sbyte.MaxValue) << shift;
                shift += VlqBitShift2;

                if ((b & MostSignificantBit2) == 0)
                {
                    byteSpan = byteSpan.Slice(index);
                    return count;
                }
            }

            throw new FormatException("Unable to read the 7-bit encoded integer");
        }
    }
}