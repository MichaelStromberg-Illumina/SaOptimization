using NirvanaCommon;
using Version7.Data;

namespace CreateGnomadVersion7
{
    public sealed class SaEntry
    {
        // temporary
        public readonly int Position;
        
        public readonly ulong       PostionAllele;
        public readonly GnomadEntry Gnomad;

        public SaEntry(int position, ulong postionAllele, GnomadEntry gnomad)
        {
            Position      = position;
            PostionAllele = postionAllele;
            Gnomad        = gnomad;
        }

        public void Write(ExtendedBinaryWriter writer)
        {
            writer.Write(PostionAllele);
            Gnomad.Write(writer);
        }
    }
}