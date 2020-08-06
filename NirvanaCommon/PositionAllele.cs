using System.Runtime.CompilerServices;

namespace NirvanaCommon
{
    public static class PositionAllele
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Convert(int position, string allele, VariantType variantType) =>
            ((ulong) position << 36) | ((ulong) Murmur.String(allele) << 3) | ((ulong) variantType & 0xf);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetPosition(ulong positionAllele) => (int) (positionAllele >> 36);
    }
}