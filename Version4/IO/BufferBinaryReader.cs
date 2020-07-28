using System;
using System.Text;

namespace Version4.IO
{
    public sealed class BufferBinaryReader
    {
        private readonly byte[] _buffer;
        private int _bufferPos;
        
        private const int MinBufferSize = 128;

        private readonly Encoding _encoding   = Encoding.UTF8;
        private readonly Decoder  _decoder    = Encoding.UTF8.GetDecoder();
        private          char[]   _charBuffer = new char[MinBufferSize];

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

        public byte ReadByte() => _buffer[_bufferPos++];

        public string ReadString()
        {
            int numBytes = ReadOptInt32();
            if (numBytes == 0) return string.Empty;
            
            int maxBufferSize = _encoding.GetMaxCharCount(numBytes);
            if (maxBufferSize > _charBuffer.Length) _charBuffer = new char[maxBufferSize];
            
            int numChars = _decoder.GetChars(_buffer, _bufferPos, numBytes, _charBuffer, 0);
            var value    = new string(_charBuffer, 0, numChars);
            _bufferPos += numBytes;
            
            return value;
        }
        
        public void SkipString()
        {
            int numBytes = ReadOptInt32();
            if (numBytes == 0) return;
            
            _bufferPos += numBytes;
        }

        public void SkipByte() => _bufferPos++;

        public byte[] ReadBytes(int numBytes)
        {
            var bytes = new byte[numBytes];
            Array.Copy(_buffer, _bufferPos, bytes, 0, numBytes);
            _bufferPos += numBytes;
            return bytes;
        }
    }
}