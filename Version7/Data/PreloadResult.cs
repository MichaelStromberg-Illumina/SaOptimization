namespace Version7.Data
{
    public class PreloadResult
    {
        public readonly ulong  PositionAllele;
        public readonly GnomadReadEntry Gnomad;

        public PreloadResult(ulong positionAllele, GnomadReadEntry gnomad)
        {
            PositionAllele = positionAllele;
            Gnomad         = gnomad;
        }
    }
}