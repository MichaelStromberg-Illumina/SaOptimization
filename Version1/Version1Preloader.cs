using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Compression.Data;
using NirvanaCommon;
using Version1.Data;
using Version1.IO;

namespace Version1
{
    public static class V1Preloader
    {
        public static int Preload(Chromosome chromosome, List<int> positions, string commonThreshold)
        {
            string saPath    = $"E:\\Data\\Nirvana\\NewSA\\gnomad_chr1_v1_{commonThreshold}.nsa";
            string indexPath = saPath + ".idx";
            
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