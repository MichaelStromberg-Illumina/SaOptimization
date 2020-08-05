using System;
using System.Runtime.InteropServices;

namespace Benchmarks.HashAlgorithms
{
    public static class Murmur
    {
        private const uint Seed = 104729;
        
        private static readonly byte[] Buffer = new byte[8];

        public static uint Long(long value)
        {
            Buffer[0] = (byte) (value >> 56);
            Buffer[1] = (byte) (value >> 48);
            Buffer[2] = (byte) (value >> 40);
            Buffer[3] = (byte) (value >> 32);
            Buffer[4] = (byte) (value >> 24);
            Buffer[5] = (byte) (value >> 16);
            Buffer[6] = (byte) (value >> 8);
            Buffer[7] = (byte) value;
            return Hash32(Buffer);
        }
        
        public static uint Hash32(byte[] bytes)
        {
            uint h1       = Seed;
            int  numBytes = bytes?.Length ?? 0;

            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;

            // marshalling does not pad out the number of bytes
            ReadOnlySpan<uint> uintSpan = MemoryMarshal.Cast<byte, uint>(bytes);

            // body
            uint k1;
            int  numInts = uintSpan.Length;

            for (int i = 0; i < numInts; i++)
            {
                k1 = uintSpan[i];

                k1 *= c1;
                k1 =  (k1 << 15) | (k1 >> 17);
                k1 *= c2;

                h1 ^= k1;
                h1 =  (h1 << 13) | (h1 >> 19);
                h1 =  h1 * 5 + 0xe6546b64;
            }

            // tail
            int numRemainingBytes = numBytes & 3;

            if (numRemainingBytes > 0)
            {
                ReadOnlySpan<byte> byteSpan = bytes.AsSpan(numBytes - numRemainingBytes);

#pragma warning disable 8509
                k1 = numRemainingBytes switch
#pragma warning restore 8509
                {
                    3 => (uint) byteSpan[2] << 16 | (uint) byteSpan[1] << 8 | (uint) byteSpan[0],
                    2 => (uint) byteSpan[1] << 8                            | (uint) byteSpan[0],
                    1 => byteSpan[0]
                };
                
                k1 *= c1;
                k1 =  (k1 << 15) | (k1 >> 17);
                k1 *= c2;
                h1 ^= k1;
            }

            // finalization
            h1 ^= (uint) numBytes;
            h1 ^= h1 >> 16;
            h1 *= 0x85ebca6b;
            h1 ^= h1 >> 13;
            h1 *= 0xc2b2ae35;
            h1 ^= h1 >> 16;

            return h1;
        }
    }
}