using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Compression.Data;
using NirvanaCommon;
using Version4.Data;
using Version4.IO;

namespace Version4
{
    public static class V4Preloader
    {
        public static int Preload(Chromosome chromosome, string saPath, string indexPath, List<int> positions, LongHashTable positionAlleles)
        {
            List<PreloadResult> results;

            var preloadBitArray = new BitArray(chromosome.Length);
            foreach (int position in positions) preloadBitArray.Set(position);

            var block   = new Block(null, 0, 0);
            var context = new ZstdContext(CompressionMode.Decompress);

            using (FileStream saStream  = FileUtilities.GetReadStream(saPath))
            using (FileStream idxStream = FileUtilities.GetReadStream(indexPath))
            using (var saReader         = new AlleleFrequencyReader(saStream, block, context))
            using (var indexReader      = new IndexReader(idxStream, block, context))
            {
                ChromosomeIndex index        = indexReader.Load(chromosome);
                IndexEntry[]    indexEntries = index.GetIndexEntries(positions);

                string[] alleles = saReader.GetAlleles(index.AlleleIndexOffset);
                results = saReader.GetAnnotatedVariants(indexEntries, preloadBitArray, positions.Count, alleles, positionAlleles);
            }

            return results.Count;
        }
    }
}