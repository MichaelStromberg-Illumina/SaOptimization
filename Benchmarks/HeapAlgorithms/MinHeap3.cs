using System;
using System.Collections.Generic;

namespace Benchmarks.HeapAlgorithms
{
    public sealed class MinHeap3<T>
    {
        private readonly List<T>         _entries;
        private readonly Func<T, T, int> _comparerFunc;

        public MinHeap3(Func<T, T, int> comparerFunc)
        {
            _entries      = new List<T>();
            _comparerFunc = comparerFunc;
        }

        private static int Parent(int i) => (i - 1) / 2;
        private static int Left(int i)   => 2 * i + 1;
        private static int Right(int i)  => 2 * i + 2;

        public void Add(T item)
        {
            _entries.Add(item);
            int i = _entries.Count - 1;

            while (i != 0 && _comparerFunc(_entries[Parent(i)], _entries[i]) > 0)
            {
                int parent = Parent(i);
                SwapItems(_entries, i, parent);
                i = parent;
            }
        }

        public T RemoveMin()
        {
            int numEntries = _entries.Count;
            if (numEntries == 1)
            {
                T last = _entries[0];
                _entries.RemoveAt(0);
                return last;
            }

            T min = _entries[0];
            _entries[0] = _entries[numEntries - 1];
            _entries.RemoveAt(numEntries - 1);
            MinHeapify(0);

            return min;
        }

        private void MinHeapify(int i)
        {
            while (true)
            {
                int l        = Left(i);
                int r        = Right(i);
                int smallest = i;

                if (l < _entries.Count && _comparerFunc(_entries[l], _entries[i])        < 0) smallest = l;
                if (r < _entries.Count && _comparerFunc(_entries[r], _entries[smallest]) < 0) smallest = r;
                if (smallest == i) return;

                SwapItems(_entries, i, smallest);
                i = smallest;
            }
        }

        private static void SwapItems(List<T> list, int i, int j)
        {
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        public T   Minimum => _entries[0];
        public int Count   => _entries.Count;
    }
}