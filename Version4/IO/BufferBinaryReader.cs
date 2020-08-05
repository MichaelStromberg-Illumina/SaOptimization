using System;

namespace Version4.IO
{
    public sealed class BufferBinaryReader
    {
        private readonly byte[] _buffer;
        private          int    _bufferPos;

        public BufferBinaryReader(byte[] buffer) => _buffer = buffer;

        private const int MostSignificantBit = 128;
        private const int VlqBitShift        = 7;

        public int ReadOptInt32()
        {
            var count = 0;
            var shift = 0;

            while (shift != 35)
            {
                byte b = _buffer[_bufferPos++];
                count |= (b & sbyte.MaxValue) << shift;
                shift += VlqBitShift;

                if ((b & MostSignificantBit) == 0) return count;
            }

            throw new FormatException("Unable to read the 7-bit encoded integer");
        }

        public long ReadOptInt64()
        {
            long count = 0;
            var  shift = 0;

            while (shift != 70)
            {
                byte b = _buffer[_bufferPos++];
                count |= (long) (b & sbyte.MaxValue) << shift;
                shift += VlqBitShift;

                if ((b & MostSignificantBit) == 0) return count;
            }

            throw new FormatException("Unable to read the 7-bit encoded long");
        }

        public Span<byte> ReadBytes(int numBytes)
        {
            Span<byte> byteSpan = _buffer.AsSpan(_bufferPos, numBytes);
            _bufferPos += numBytes;
            return byteSpan;
        }
    }
}