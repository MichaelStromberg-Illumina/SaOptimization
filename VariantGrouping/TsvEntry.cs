namespace VariantGrouping
{
    public sealed class TsvEntry
    {
        public readonly int Position;
        public readonly string RefAllele;
        public readonly string AltAllele;
        public readonly string Json;
        public readonly bool IsCommon;

        public TsvEntry(int position, string refAllele, string altAllele, string json, bool isCommon)
        {
            Position  = position;
            RefAllele = refAllele;
            AltAllele = altAllele;
            Json      = json;
            IsCommon  = isCommon;
        }
    }
}