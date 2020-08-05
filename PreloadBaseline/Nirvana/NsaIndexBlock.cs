using System;

namespace PreloadBaseline.Nirvana
{
    public sealed class NsaIndexBlock
    {
        public readonly int Start;
        public readonly int End;
        public readonly long FilePosition;

        [Obsolete("Use a factory method instead of an extra constructor.")]
        public NsaIndexBlock(ExtendedBinaryReader reader)
        {
            Start        = reader.ReadOptInt32();
            End          = reader.ReadOptInt32();
            FilePosition = reader.ReadOptInt64();
            reader.ReadOptInt32();
        }

        public int CompareTo(int position)
        {
            if (Start <= position && position <= End) return 0;
            return Start.CompareTo(position);
        }
    }
}