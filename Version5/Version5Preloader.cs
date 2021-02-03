using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Compression.Data;
using NirvanaCommon;
using Version5.Data;
using Version5.IO;

namespace Version5
{
    public static class V5Preloader
    {
        public static int Preload(Chromosome chromosome, string saPath, string indexPath, ulong[] positionAlleles,
            LongHashTable positionAlleleSet)
        {
            List<PreloadResult> results;

            var block   = new Block(null, 0, 0);
            var context = new ZstdContext(CompressionMode.Decompress);

            using (FileStream saStream = FileUtilities.GetReadStream(saPath))
            using (FileStream idxStream = FileUtilities.GetReadStream(indexPath))
            using (var saReader = new AlleleFrequencyReader(saStream, block, context))
            using (var indexReader = new IndexReader(idxStream, block, context))
            {
                ChromosomeIndex index                   = indexReader.Load(chromosome);
                List<ulong>     filteredPositionAlleles = index.Filter(positionAlleles);
                IndexEntry[]    indexEntries            = index.GetIndexEntries(filteredPositionAlleles);
                string[]        alleles                 = saReader.GetAlleles(index.AlleleIndexOffset);
                
                results = saReader.GetAnnotatedVariants(indexEntries, positionAlleleSet, filteredPositionAlleles.Count,
                    alleles);
            }

            return results.Count;
        }
    }
}