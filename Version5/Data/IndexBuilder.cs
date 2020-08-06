using System.Collections.Generic;
using NirvanaCommon;

namespace Version5.Data
{
    public class IndexBuilder
    {
        private long _alleleIndexOffset;
        private readonly ChromosomeIndex[] _chromsomeIndices;
        private readonly List<IndexEntry>  _commonEntries = new List<IndexEntry>();
        private readonly List<IndexEntry>  _rareEntries   = new List<IndexEntry>();
        
        public IndexBuilder(int numRefSeqs)
        {
            _chromsomeIndices = new ChromosomeIndex[numRefSeqs];
        }

        public ChromosomeIndex[] ChromosomeIndices => _chromsomeIndices;

        public void AddBlock(int lastPosition, long fileOffset, bool useCommon)
        {
            var indexEntry = new IndexEntry(lastPosition, fileOffset);
            if (useCommon) _commonEntries.Add(indexEntry);
            else _rareEntries.Add(indexEntry);
        }

        public void FinalizeChromosome(ushort refIndex, Xor8 xorFilter, ulong[] commonPositionAlleles)
        {
            var commonHash = new LongHashTable();
            foreach (ulong allele in commonPositionAlleles) commonHash.Add(allele);
            
            _chromsomeIndices[refIndex] =
                new ChromosomeIndex(xorFilter, commonHash, _commonEntries.ToArray(), _rareEntries.ToArray(), _alleleIndexOffset);
            _commonEntries.Clear();
            _rareEntries.Clear();
        }

        public void SetAlleleIndexOffset(in long fileOffset) => _alleleIndexOffset = fileOffset;
    }
}