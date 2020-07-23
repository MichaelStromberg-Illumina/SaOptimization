using System;

namespace Benchmarks.HeapAlgorithms
{
    public static class SortUtils
    {
        public static void Shuffle<T>(T[] entries)
        {
            var r          = new Random(23);
            int numEntries = entries.Length;

            for (int i = numEntries - 1; i > 0; i--)
            {
                int j = r.Next(0, i + 1);

                T temp = entries[i];
                entries[i] = entries[j];
                entries[j] = temp;
            }
        }
    }
}