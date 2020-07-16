using System;
using Version1.Nirvana;

namespace Version1.Utilities
{
    public static class VariantTypeUtils
    {
        public static VariantType GetVariantType(string refAllele, string altAllele)
        {
            int referenceAlleleLen = refAllele.Length;
            int alternateAlleleLen = altAllele.Length;

            if (alternateAlleleLen != referenceAlleleLen)
            {
                if (alternateAlleleLen == 0 && referenceAlleleLen > 0) return VariantType.deletion;
                if (alternateAlleleLen > 0 && referenceAlleleLen == 0) return VariantType.insertion;

                return VariantType.indel;
            }

            VariantType variantType = alternateAlleleLen == 1 ? VariantType.SNV : VariantType.MNV;

            return variantType;
        }
        
        public static void CheckVariantType(VariantType variantType)
        {
            switch (variantType)
            {
                case VariantType.deletion:
                case VariantType.insertion:
                case VariantType.SNV:
                case VariantType.MNV:
                    return;
                default:
                    throw new NotSupportedException($"Encountered an unsupported variant type: {variantType}");
            }
        }
    }
}