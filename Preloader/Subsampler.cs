using System;
using System.Collections.Generic;
using System.Linq;
using NirvanaCommon;

namespace Preloader
{
    public static class Subsampler
    {
        public static VcfPreloadData Subsample(List<string> shuffledLines, int numPositions)
        {
            // grab the subset
            var newEntries = new List<SampleEntry>(numPositions);
            for (var i = 0; i < numPositions; i++)
            {
                string   line        = shuffledLines[i];
                string[] cols        = line.Split('\t', 2);
                int      position    = int.Parse(cols[0]);
                var      sampleEntry = new SampleEntry(position, line);
                newEntries.Add(sampleEntry);
            }

            // sort the lines by position
            var newLines = new List<string>(numPositions);
            foreach (SampleEntry entry in newEntries.OrderBy(x => x.Position)) newLines.Add(entry.Line);

            // process the sorted list
            return Preloader.GetPositions(newLines, false);
        }

        private class SampleEntry
        {
            public readonly int    Position;
            public readonly string Line;

            public SampleEntry(int position, string line)
            {
                Position = position;
                Line     = line;
            }
        }
        
        public static void Shuffle(this List<string> entryList)
        {
            var r          = new Random(11);
            int numEntries = entryList.Count;

            for (int i = numEntries - 1; i > 0; i--)
            {
                int j = r.Next(0, i + 1);

                string temp = entryList[i];
                entryList[i] = entryList[j];
                entryList[j] = temp;
            }
        }
    }
}