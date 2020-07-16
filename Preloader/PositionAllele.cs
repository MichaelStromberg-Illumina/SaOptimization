namespace Preloader
{
    public sealed class PositionAllele
    {
        public readonly int Position;
        public readonly string RefAllele;
        public readonly string AltAllele;

        public PositionAllele(int position, string refAllele, string altAllele)
        {
            Position  = position;
            RefAllele = refAllele;
            AltAllele = altAllele;
        }
    }
}