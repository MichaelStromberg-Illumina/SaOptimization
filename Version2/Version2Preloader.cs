using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Compression.Data;
using NirvanaCommon;
using Version2.Data;
using Version2.IO;

namespace Version2
{
    public static class V2Preloader
    {
        public static int Preload(Chromosome chromosome, List<int> positions)
        {
            List<PreloadResult> results;

            var preloadBitArray = new BitArray(chromosome.Length);
            foreach (int position in positions) preloadBitArray.Set(position);

            var block   = new Block(null, 0, 0);
            var context = new ZstdContext(CompressionMode.Decompress);

            using (FileStream saStream  = FileUtilities.GetReadStream(SaConstants.SaPath))
            using (FileStream idxStream = FileUtilities.GetReadStream(SaConstants.IndexPath))
            using (var saReader         = new AlleleFrequencyReader(saStream, block, context))
            using (var indexReader      = new IndexReader(idxStream, block, context, saReader.Dictionary))
            {
                ChromosomeIndex index        = indexReader.Load(chromosome);
                IndexEntry[]    indexEntries = index.GetIndexEntries(positions);

                results = saReader.GetAnnotatedVariants(indexEntries, preloadBitArray, positions.Count);
            }

            return results.Count;
        }
    }
}