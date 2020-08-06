using System.IO;
using System.Text;

namespace NirvanaCommon
{
    public class ExtendedBinaryWriter : BinaryWriter
    {
        public ExtendedBinaryWriter(Stream output, bool leaveOpen = false)
            : base(output, Encoding.Default, leaveOpen)
        {
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
        
        public void WriteOpt(ulong num)
        {
            while (num >= 128U)
            {
                Write((byte) (num | 128U));
                num >>= 7;
            }

            Write((byte) num);
        }
    }
}