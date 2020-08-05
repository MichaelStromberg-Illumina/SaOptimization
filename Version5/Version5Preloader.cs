﻿using System.Collections.Generic;
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
        public static int Preload(Chromosome chromosome, List<int> positions, LongHashTable positionAlleles)
        {
            List<PreloadResult> results;

            var preloadBitArray = new BitArray(chromosome.Length);
            foreach (int position in positions) preloadBitArray.Set(position);

            var block   = new Block(null, 0, 0);
            var context = new ZstdContext(CompressionMode.Decompress);

            using (FileStream saStream  = FileUtilities.GetReadStream(SaConstants.SaPath))
            using (FileStream idxStream = FileUtilities.GetReadStream(SaConstants.IndexPath))
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