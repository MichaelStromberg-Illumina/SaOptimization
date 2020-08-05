using System.Runtime.CompilerServices;

namespace NirvanaCommon
{
    public static class PositionAllele
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Convert(int position, string allele, VariantType variantType) =>
            ((long) position << 36) | ((long) Murmur.String(allele) << 3) | ((long) variantType & 0xf);
    }
}