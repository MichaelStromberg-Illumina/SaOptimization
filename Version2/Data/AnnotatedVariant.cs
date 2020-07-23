using NirvanaCommon;

namespace Version2.Data
{
    public sealed class AnnotatedVariant
    {
        public readonly int         Position;
        public readonly VariantType VariantType;
        public readonly string      Allele;
        public readonly string      Json;

        public AnnotatedVariant(int position, VariantType variantType, string allele, string json)
        {
            Position    = position;
            VariantType = variantType;
            Allele      = allele;
            Json        = json;
        }
    }
}