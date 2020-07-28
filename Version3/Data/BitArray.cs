using System;
using System.Runtime.CompilerServices;

namespace Version3.Data
{
    public sealed class BitArray
    {
        private readonly int   _maxPosition;
        private readonly int[] _data;

        private const int BitShiftPerInt32 = 5;

        public BitArray(int[] data)
        {
            _maxPosition = data.Length * sizeof(int) * 10;
            //Console.WriteLine($"CTR1: max position: {_maxPosition:N0}");
            _data        = data;
        }
        
        public BitArray(int maxPosition)
        {
            //Console.WriteLine($"CTR2: max position: {maxPosition:N0}");
            _maxPosition = maxPosition;
            _data        = new int[GetInt32ArrayLengthFromMaxPosition(maxPosition - 1)];
        }

        private static int GetInt32ArrayLengthFromMaxPosition(int n) =>
            (int) ((uint) (n - 1 + (1 << BitShiftPerInt32)) >> BitShiftPerInt32);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int position)
        {
            if (position > _maxPosition)
            {
                Console.WriteLine($"Set: position: {position:N0} vs max position: {_maxPosition:N0}");
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            int     index   = position - 1;
            int     bitMask = 1 << index;
            ref int segment = ref _data[index >> 5];
            segment |= bitMask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Get(int position)
        {
            if (position > _maxPosition)
            {
                Console.WriteLine($"Get: position: {position:N0} vs max position: {_maxPosition:N0}");
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            int index = position - 1;
            return (_data[index >> 5] & (1 << index)) != 0;
        }

        public int[] Data => _data;
    }
}