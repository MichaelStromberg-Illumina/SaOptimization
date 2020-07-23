using System;

namespace Benchmarks.BitArrayAlgorithms
{
    public sealed class BitArray1
    {
        private readonly int   _maxPosition;
        private readonly int[] _data;
        
        private const int BitShiftPerInt32 = 5;

        public BitArray1(int maxPosition)
        {
            _maxPosition = maxPosition;
            _data        = new int[GetInt32ArrayLengthFromMaxPosition(maxPosition - 1)];
        }

        private static int GetInt32ArrayLengthFromMaxPosition(int n) =>
            (int) ((uint) (n - 1 + (1 << BitShiftPerInt32)) >> BitShiftPerInt32);

        public void Set(int position)
        {
            if (position > _maxPosition) throw new ArgumentOutOfRangeException(nameof(position));

            Console.WriteLine($"position: {position}");
            
            int     index   = position - 1;
            Console.WriteLine($"index: {index}");
            
            byte     bitMask = (byte)(1 << index);
            var binaryBitMask = Convert.ToString((uint)bitMask,  2);
            var hexBitMask = Convert.ToString((uint)bitMask, 16);
            Console.WriteLine($"bitmask: {(uint)bitMask} ({binaryBitMask} - {hexBitMask})");
            
            ref int segment = ref _data[index >> 5];
            Console.WriteLine($"int index: {index >> 5}\n");
            
            segment |= bitMask;
        }
        
        public bool Get(int position)
        {
            if (position > _maxPosition) throw new ArgumentOutOfRangeException(nameof(position));
            int index = position - 1;
            return (_data[index >> 5] & (1 << index)) != 0;
        }

        public int[] Data => _data;
    }
}