using System.Collections.Generic;
using NirvanaCommon;
using Xunit;

namespace UnitTests.NirvanaCommon
{
    public sealed class MurmurTests
    {
        public static IEnumerable<object[]> Data => new List<object[]>
        {
            new object[] {null, 0, 0},
            new object[] {null, 1, 0x514E28B7},
            new object[] {null, 0xffffffff, 0x81F16F39},
            new object[] {new byte[] {0xff, 0xff, 0xff, 0xff}, 0, 0x76293B50},
            new object[] {new byte[] {0x21, 0x43, 0x65, 0x87}, 0, 0xF55B516B},
            new object[] {new byte[] {0x21, 0x43, 0x65, 0x87}, 0x5082EDEE, 0x2362F9DE},
            new object[] {new byte[] {0x21, 0x43, 0x65}, 0, 0x7E4A8634},
            new object[] {new byte[] {0x21, 0x43}, 0, 0xA0F7B07A},
            new object[] {new byte[] {0x21}, 0, 0x72661CF4},
            new object[] {new byte[] {0x00, 0x00, 0x00, 0x00}, 0, 0x2362F9DE},
            new object[] {new byte[] {0x00, 0x00, 0x00}, 0, 0x85F0B427},
            new object[] {new byte[] {0x00, 0x00}, 0, 0x30F4C306},
            new object[] {new byte[] {0x00}, 0, 0x514E28B7}
        };

        [Theory]
        [MemberData(nameof(Data))]
        public void Hash32(byte[] input, uint seed, uint expectedResult)
        {
            uint observedResult = Murmur.Hash32(input, seed);
            Assert.Equal(expectedResult, observedResult);
        }
    }
}