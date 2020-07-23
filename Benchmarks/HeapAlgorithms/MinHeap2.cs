using System;
using System.Collections.Generic;

namespace Benchmarks.HeapAlgorithms
{
    public sealed class MinHeap2<T>
    {
        private readonly List<T>         _entries;
        private readonly Func<T, T, int> _comparerFunc;

        public MinHeap2(Func<T, T, int> comparerFunc)
        {
            _entries      = new List<T>();
            _comparerFunc = comparerFunc;
        }

        public void Add(T item)
        {
            _entries.Add(item);

            int currentIndex = _entries.Count - 1;

            while (currentIndex > 0)
            {
                int parentIndex = currentIndex % 2 == 0 ? currentIndex / 2 - 1 : currentIndex / 2;

                if (_comparerFunc(_entries[currentIndex], _entries[parentIndex]) < 0)
                    SwapItems(_entries, currentIndex, parentIndex);
                currentIndex = parentIndex;
            }
        }

        public T RemoveMin()
        {
            T min = _entries[0];

            // the last item form the array is brought to the root and pushed down to the appropriate position
            int numEntries = _entries.Count;
            _entries[0] = _entries[numEntries - 1];
            _entries.RemoveAt(numEntries - 1);
            numEntries--;

            for (var i = 0; i < numEntries / 2;)
            {
                int j = 2 * i + 1;

                if (j + 1 < numEntries && _comparerFunc(_entries[j], _entries[j + 1]) > 0)
                    // both children are present
                    j++; //A[2*i+2] is the smaller child

                if (_comparerFunc(_entries[i], _entries[j]) > 0) SwapItems(_entries, i, j);
                i = j;
            }

            return min;
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