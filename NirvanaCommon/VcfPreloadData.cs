using System.Collections.Generic;

namespace NirvanaCommon
{
    public class VcfPreloadData
    {
        public readonly List<int>     Positions;
        public readonly LongHashTable PositionAlleleHashTable;
        public readonly ulong[]       PositionAlleles;

        public VcfPreloadData(List<int> positions, LongHashTable positionAlleleHashTable, ulong[] positionAlleles)
        {
            Positions               = positions;
            PositionAlleleHashTable = positionAlleleHashTable;
            PositionAlleles         = positionAlleles;
        }
    }
}