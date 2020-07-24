using System;
using System.IO;
using System.Text;

namespace NirvanaCommon
{
    public sealed class ExtendedBinaryReader : BinaryReader
    {
        private readonly Stream _stream;

        public ExtendedBinaryReader(Stream stream, bool leaveOpen = false)
            : base(stream, Encoding.Default, leaveOpen)
        {
            _stream = stream;
        }

        public int ReadOptInt32()
        {
            var count = 0;
            var shift = 0;

            while (shift != 35)
            {
                byte b = ReadByte();
                count |= (b & sbyte.MaxValue) << shift;
                shift += 7;

                if ((b & 128) == 0) return count;
            }

            throw new FormatException("Unable to read the 7-bit encoded integer");
        }

        public long ReadOptInt64()
        {
            long count = 0;
            var  shift = 0;

            while (shift != 70)
            {
                byte b = ReadByte();
                count |= (long) (b & sbyte.MaxValue) << shift;
                shift += 7;

                if ((b & 128) == 0) return count;
            }

            throw new FormatException("Unable to read the 7-bit encoded long");
        }

        public void ReadOptBytes(byte[] buffer, int numBytes) => _stream.Read(buffer, 0, numBytes);
    }
}