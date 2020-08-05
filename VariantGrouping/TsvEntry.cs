namespace VariantGrouping
{
    public sealed class TsvEntry
    {
        public readonly int Position;
        public readonly string RefAllele;
        public readonly string AltAllele;
        public readonly string Json;

        public TsvEntry(int position, string refAllele, string altAllele, string json)
        {
            Position  = position;
            RefAllele = refAllele;
            AltAllele = altAllele;
            Json      = json;
        }
    }
}