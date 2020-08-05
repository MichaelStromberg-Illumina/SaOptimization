using System.Runtime.CompilerServices;

namespace NirvanaCommon
{
    public static class Seed
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SplitMix64(ulong x)
        {
            x += 0x9e3779b97f4a7c15;
            x =  (x ^ (x >> 30)) * 0xbf58476d1ce4e5b9;
            x =  (x ^ (x >> 27)) * 0x94d049bb133111eb;
            return x ^ (x >> 31);
        }
    }
}