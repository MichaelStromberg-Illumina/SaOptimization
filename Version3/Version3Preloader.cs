using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Compression.Data;
using NirvanaCommon;
using Version3.Data;
using Version3.IO;

namespace Version3
{
    public static class V3Preloader
    {
        public static int Preload(Chromosome chromosome, string saPath, string indexPath, List<int> positions)
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

                results = saReader.GetAnnotatedVariants(indexEntries, preloadBitArray, positions.Count);
            }

            return results.Count;
        }
    }
}