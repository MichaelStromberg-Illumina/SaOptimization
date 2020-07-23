using NirvanaCommon;

namespace VariantGrouping
{
    public readonly struct PreloadVariant
    {
        public readonly int         Position;
        public readonly string      Allele;
        public readonly VariantType VariantType;

        public PreloadVariant(int position, string allele, VariantType variantType)
        {
            Position    = position;
            Allele      = allele;
            VariantType = variantType;
        }
    }
}