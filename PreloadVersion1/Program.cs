using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Compression.Data;
using JetBrains.Profiler.Api;
using NirvanaCommon;
using VariantGrouping;
using Version1.Data;
using Version1.IO;

namespace PreloadVersion1
{
    class Program
    {
        static void Main()
        {
            MemoryProfiler.CollectAllocations(true);
            MemoryProfiler.GetSnapshot();
            
            const string saPath    = @"E:\Data\Nirvana\NewSA\gnomad_chr1.nsa";
            const string indexPath = saPath + ".idx";

            var chr1 = new Chromosome("chr1", "1", null, null, 249250621, 0);

            // var variants = new List<PreloadVariant>
            // {
            //     new PreloadVariant(10000,    "G",                 VariantType.SNV),       // not in data source
            //     new PreloadVariant(10061,    "ACCCTAACCCTAACCCT", VariantType.deletion),  // rare 0
            //     new PreloadVariant(10078,    "A",                 VariantType.insertion), // rare 0
            //     new PreloadVariant(10147,    "C",                 VariantType.SNV),       // common 0, rare 0
            //     new PreloadVariant(10508,    "G",                 VariantType.SNV),       // rare 1
            //     new PreloadVariant(1138431,  "C",                 VariantType.SNV),       // rare 936
            //     new PreloadVariant(23492354, "A",                 VariantType.SNV)        // common 1
            // };

            List<int> tempPositions = Preloader.Preloader.GetPositions(@"E:\Data\Nirvana\gnomAD_chr1_preload.tsv");

            List<int> positions = Subsampler.Subsample(tempPositions, 1);
            
            var variants = new List<PreloadVariant>(positions.Count);
            foreach (var position in positions) variants.Add(new PreloadVariant(position, null, VariantType.SNV));
            
            var preloadBitArray = new BitArray(chr1.Length);
            foreach(PreloadVariant variant in variants) preloadBitArray.Set(variant.Position);

            var benchmark = new Benchmark();
            
            using (FileStream saStream  = FileUtilities.GetReadStream(saPath))
            using (FileStream idxStream = FileUtilities.GetReadStream(indexPath))
            using (var saReader         = new AlleleFrequencyReader(saStream))
            using (var indexReader      = new IndexReader(idxStream))
            {
                ChromosomeIndex index       = indexReader.Load(chr1);
                BlockRange[]    blockRanges = index.GetBlockRanges(variants);

                var context = new ZstdContext(CompressionMode.Decompress);

                // Console.WriteLine();
                // Console.WriteLine("Final block ranges:");
                // foreach (BlockRange blockRange in blockRanges)
                // {
                //     Console.WriteLine($"- file offset: {blockRange.FileOffset:N0}, # of blocks: {blockRange.NumBlocks:N0}");
                // }

                saReader.GetAnnotatedVariants(blockRanges, preloadBitArray, variants, context, saReader.Dictionary);
            }
            
            ShowElapsedTime(benchmark);
            MemoryProfiler.GetSnapshot();
            MemoryUtilities.PrintAllocations();
        }
        
        private static void ShowElapsedTime(Benchmark benchmark)
        {
            Console.WriteLine($"  - elapsed time: {benchmark.GetElapsedTime()}");
            Console.WriteLine($"  - current RAM:  {MemoryUtilities.GetCurrentMemoryUsage()}");
            Console.WriteLine($"  - peak RAM:     {MemoryUtilities.GetPeakMemoryUsage()}\n");
        }
    }
}