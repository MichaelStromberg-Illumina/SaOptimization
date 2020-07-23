// using System;
// using System.Runtime.CompilerServices;
//
// namespace Benchmarks.BitArrayAlgorithms
// {
//     public sealed class BitArray_Inline_Byte
//     {
//         private readonly int   _maxPosition;
//         private readonly byte[] _data;
//
//         private const int BitShiftPerInt32 = 5;
//         private const int BitShiftPerByte  = 3;
//
//         public BitArray_Inline_Byte(int maxPosition)
//         {
//             _maxPosition = maxPosition;
//             _data        = new byte[GetByteArrayLengthFromBitLength(maxPosition - 1)];
//         }
//
//         private static int GetByteArrayLengthFromBitLength(int n) =>
//             (int) ((uint) (n - 1 + (1 << BitShiftPerByte)) >> BitShiftPerByte);
//
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         public void Set(int position)
//         {
//             if (position > _maxPosition) throw new ArgumentOutOfRangeException(nameof(position));
//
//             int     index   = position - 1;
//             byte     bitMask = 1 << index;
//             ref byte segment = ref _data[index >> 5];
//             segment |= bitMask;
//         }
//         
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         public bool Get(int position)
//         {
//             if (position > _maxPosition) throw new ArgumentOutOfRangeException(nameof(position));
//             int index = position - 1;
//             return (_data[index >> 5] & (1 << index)) != 0;
//         }
//
//         public int[] Data => _data;
//     }
// }