using System.Collections.Generic;

namespace Version3.Data
{
    public class IndexBuilder
    {
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

        public void FinalizeChromosome(ushort refIndex)
        {
            _chromsomeIndices[refIndex] =
                new ChromosomeIndex(_commonEntries.ToArray(), _rareEntries.ToArray());
            _commonEntries.Clear();
            _rareEntries.Clear();
        }
    }
}