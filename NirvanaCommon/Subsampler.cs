using System;
using System.Collections.Generic;
using System.Linq;

namespace NirvanaCommon
{
    public static class Subsampler
    {
        public static List<int> Subsample(List<int> positions, int numPositions)
        {
            var newPositions = new List<int>(numPositions);

            positions.Shuffle();
            for (int i = 0; i < numPositions; i++) newPositions.Add(positions[i]);

            return newPositions.OrderBy(x => x).ToList();
        }
        
        private static void Shuffle(this List<int> entryList)
        {
            var r          = new Random(23);
            int numEntries = entryList.Count;

            for (int i = numEntries - 1; i > 0; i--)
            {
                int j = r.Next(0, i + 1);

                int temp = entryList[i];
                entryList[i] = entryList[j];
                entryList[j] = temp;
            }
        }
    }
}