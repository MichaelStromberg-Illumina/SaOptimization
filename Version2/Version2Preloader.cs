using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Compression.Data;
using NirvanaCommon;
using VariantGrouping;
using Version2.Data;
using Version2.IO;

namespace Version2
{
    public sealed class V2Preloader
    {
        public static int Preload(Chromosome chromosome, List<PreloadVariant> variants)
        {
            const string saPath    = @"E:\Data\Nirvana\NewSA\gnomad_chr1.nsa";
            const string indexPath = saPath + ".idx";

            var preloadBitArray = new BitArray(chromosome.Length);
            foreach (PreloadVariant variant in variants) preloadBitArray.Set(variant.Position);

            List<AnnotatedVariant> annotatedVariants;
            
            using (FileStream saStream = FileUtilities.GetReadStream(saPath))
            using (FileStream idxStream = FileUtilities.GetReadStream(indexPath))
            using (var saReader = new AlleleFrequencyReader(saStream))
            using (var indexReader = new IndexReader(idxStream))
            {
                ChromosomeIndex index       = indexReader.Load(chromosome);
                BlockRange[]    blockRanges = index.GetBlockRanges(variants);

                var context = new ZstdContext(CompressionMode.Decompress);
                annotatedVariants = saReader.GetAnnotatedVariants(blockRanges, preloadBitArray, variants, context, saReader.Dictionary);
            }

            return annotatedVariants.Count;
        }
    }
}