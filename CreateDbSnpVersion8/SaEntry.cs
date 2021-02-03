using NirvanaCommon;
using Version8.Data;

namespace CreateDbSnpVersion8
{
    public sealed class SaEntry
    {
        // temporary
        public readonly int Position;

        public readonly ulong PostionAllele;
        public readonly int   RawRsId;

        public SaEntry(int position, ulong postionAllele, int rawRsId)
        {
            Position      = position;
            PostionAllele = postionAllele;
            RawRsId       = rawRsId;
        }

        public void Write(ExtendedBinaryWriter writer)
        {
            writer.Write(PostionAllele);
            writer.WriteOpt(RawRsId);
        }
    }
}