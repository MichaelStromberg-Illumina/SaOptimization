using System.Collections.Generic;
using System.Linq;
using NirvanaCommon;

namespace Version5.Data
{
    public sealed class ChromosomeIndex
    {
        public readonly Xor8          XorFilter;
        public readonly LongHashTable CommonPositionAlleles;
        public readonly IndexEntry[]  Common;
        public readonly IndexEntry[]  Rare;
        public readonly long          AlleleIndexOffset;

        public ChromosomeIndex(Xor8 xorFilter, LongHashTable commonHash, IndexEntry[] common, IndexEntry[] rare,
            long alleleIndexOffset)
        {
            XorFilter             = xorFilter;
            CommonPositionAlleles = commonHash;
            Common                = common;
            Rare                  = rare;
            AlleleIndexOffset     = alleleIndexOffset;
        }

        public IndexEntry[] GetIndexEntries(List<ulong> positionAlleles)
        {
            var entries = new List<IndexEntry>();

            var commonIndexes = new HashSet<int>();
            var rareIndexes   = new HashSet<int>();

            foreach (ulong positionAllele in positionAlleles)
            {
                int position = PositionAllele.GetPosition(positionAllele);

                // if (position == 1008605007091690) Console.WriteLine("BOB");

                if (CommonPositionAlleles.Contains(positionAllele)) GetIndexEntry(position, Common, commonIndexes);
                else GetIndexEntry(position,                                                Rare,   rareIndexes);
            }

            foreach (int index in commonIndexes.OrderBy(x => x)) entries.Add(Common[index]);
            foreach (int index in rareIndexes.OrderBy(x => x)) entries.Add(Rare[index]);

            // Console.WriteLine($"- GetIndexEntries: common index entries: {commonIndexes.Count:N0}, rare index entries: {rareIndexes.Count:N0}");

            return entries.ToArray();
        }

        private static void GetIndexEntry(int position, IndexEntry[] index, HashSet<int> hashSet)
        {
            int blockIndex = BinarySearch(index, position);
            if (blockIndex >= 0) hashSet.Add(blockIndex);
        }

        private static int BinarySearch(IndexEntry[] entries, int position)
        {
            var begin = 0;
            int end   = entries.Length - 1;

            while (begin <= end)
            {
                int        index      = begin + (end - begin >> 1);
                IndexEntry indexEntry = entries[index];

                int start = index == 0 ? 1 : entries[index - 1].End + 1;
                if (position >= start && position <= indexEntry.End) return index;

                if (indexEntry.End < position) begin = index + 1;
                else if (position  < start) end      = index - 1;
            }

            return ~begin;
        }

        public List<ulong> Filter(ulong[] positionAlleles)
        {
            var filteredPositions = new List<ulong>(positionAlleles.Length);

            foreach (ulong pa in positionAlleles)
                if (XorFilter.Contains(pa))
                    filteredPositions.Add(pa);

            return filteredPositions;
        }
    }
}