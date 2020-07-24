using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Compression.Data;
using NirvanaCommon;
using VariantGrouping;
using Version1.Data;
using Version1.IO;

namespace Version1
{
    public sealed class V1Preloader
    {
        public static int Preload(Chromosome chromosome, List<int> positions)
        {
            const string saPath    = @"E:\Data\Nirvana\NewSA\gnomad_chr1.nsa";
            const string indexPath = saPath + ".idx";
            
            List<PreloadResult> results;

            var preloadBitArray = new BitArray(chromosome.Length);
            foreach (int position in positions) preloadBitArray.Set(position);
            
            var block   = new Block(null, 0, 0);
            var context = new ZstdContext(CompressionMode.Decompress);

            using (FileStream saStream     = FileUtilities.GetReadStream(saPath))
            using (FileStream idxStream    = FileUtilities.GetReadStream(indexPath))
            using (var saReader            = new AlleleFrequencyReader(saStream, block, context))
            using (var indexReader         = new IndexReader(idxStream, block, context, saReader.Dictionary))
            {
                ChromosomeIndex index        = indexReader.Load(chromosome);
                IndexEntry[]    indexEntries = index.GetIndexEntries(positions);

                results = saReader.GetAnnotatedVariants(indexEntries, preloadBitArray, positions.Count);
            }

            return results.Count;
        }
    }
}