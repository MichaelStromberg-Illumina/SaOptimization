using System;

namespace Benchmarks.BitArrayAlgorithms
{
    public class ChromosomeBitMap
    {
        //can be a used or keeping track of item existance of <= 64 items 
        public           int    Count { get; private set; }
        private readonly byte[] _buffer;
        public           int    Capacity       { get; private set; }
        private          int    BufferSize     => Capacity >> 3;

        public ChromosomeBitMap(int capacity)
        {
            SetCapacity(capacity);

            _buffer = new byte[BufferSize];
        }

        private void SetCapacity(int capacity)
        {
            if ((capacity & 7) != 0)
                //not a multiple of 8. So we want to make it one.
                Capacity  = ((capacity >> 3) + 1) << 3;
            else Capacity = capacity;
        }

        public void Set(int position)
        {
            (int i, uint flag) = GetBufferIndexAndFlag(position);

            if ((_buffer[i] & flag) == 0) Count++;

            _buffer[i] |= (byte) flag;
        }

        private (int bufferIndex, uint flag) GetBufferIndexAndFlag(int position)
        {
            if (position < 1 || position > Capacity)
            {
                throw new IndexOutOfRangeException($"Position has to be between [1,{Capacity}]. Observed: {position}");
            }

            var index = position - 1;

            var bufferIndex = index >> 3; // divide by 8
            var bitIndex    = index & 7;  // modulo 8
            var flag        = (uint) 1 << bitIndex;
            return (bufferIndex, flag);
        }

        public bool IsSet(int position)
        {
            (int i, uint flag) = GetBufferIndexAndFlag(position);

            return (_buffer[i] & flag) != 0;
        }
    }
}