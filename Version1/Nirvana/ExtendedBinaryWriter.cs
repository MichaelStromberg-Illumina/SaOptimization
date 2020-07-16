using System.IO;
using System.Text;

namespace Version1.Nirvana
{
    public class ExtendedBinaryWriter : BinaryWriter
    {
        public ExtendedBinaryWriter(Stream output, bool leaveOpen = false)
            : base(output, Encoding.Default, leaveOpen)
        {
        }
        
        public void WriteOpt(ushort value)
        {
            ushort num = value;

            while (num >= 128U)
            {
                Write((byte) (num | 128U));
                num >>= 7;
            }

            Write((byte) num);
        }

        public void WriteOpt(int value)
        {
            var num = (uint) value;

            while (num >= 128U)
            {
                Write((byte) (num | 128U));
                num >>= 7;
            }

            Write((byte) num);
        }

        public void WriteOpt(long value)
        {
            var num = (ulong) value;

            while (num >= 128U)
            {
                Write((byte) (num | 128U));
                num >>= 7;
            }

            Write((byte) num);
        }

        public void WriteOptAscii(string s)
        {
            int numBytes = s?.Length ?? 0;
            WriteOpt(numBytes);

            // sanity check: handle null strings
            if (s == null) return;

            // write the ASCII bytes
            Write(Encoding.ASCII.GetBytes(s));
        }
    }
}