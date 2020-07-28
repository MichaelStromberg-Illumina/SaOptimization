using System;

namespace Version3.Data
{
    public sealed class MinHeap<T>
    {
        private T[] _entries;
        private int _numEntries;

        private const int DefaultCapacity = 32;
        private readonly Func<T, T, int> _comparerFunc;

        public MinHeap(Func<T, T, int> comparerFunc, int maxEntries = DefaultCapacity)
        {
            _entries      = new T[maxEntries];
            _comparerFunc = comparerFunc;
        }

        public void Add(T item)
        {
            int i = _numEntries;
            _numEntries++;

            if (_numEntries == _entries.Length) DoubleCapacity();
            
            _entries[i] = item;

            while (i != 0 && _comparerFunc(_entries[(i - 1) / 2], _entries[i]) > 0)
            {
                int parent = (i - 1) / 2;
                SwapItems(_entries, i, parent);
                i = parent;
            }
        }

        private void DoubleCapacity()
        {
            int newSize    = _entries.Length * 2;
            var newEntries = new T[newSize];
            Array.Copy(_entries, 0, newEntries, 0, _entries.Length);
            _entries = newEntries;
        }

        public T RemoveMin()
        {
            if (_numEntries == 1)
            {
                T last = _entries[0];
                _numEntries = 0;
                return last;
            }

            _numEntries--;
            if (_numEntries < 0) throw new InvalidOperationException("Unable to remove min item. Heap is empty.");
            
            T min = _entries[0];
            _entries[0] = _entries[_numEntries];
            MinHeapify(0);

            return min;
        }

        private void MinHeapify(int i)
        {
            while (true)
            {
                int l        = 2 * i + 1;
                int r        = 2 * i + 2;
                int smallest = i;

                if (l < _numEntries && _comparerFunc(_entries[l], _entries[i])        < 0) smallest = l;
                if (r < _numEntries && _comparerFunc(_entries[r], _entries[smallest]) < 0) smallest = r;
                if (smallest == i) return;

                SwapItems(_entries, i, smallest);
                i = smallest;
            }
        }

        private static void SwapItems(T[] list, int i, int j)
        {
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        public T   Minimum => _entries[0];
        public int Count   => _numEntries;
    }
}