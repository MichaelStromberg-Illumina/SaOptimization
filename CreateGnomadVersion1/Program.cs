using System;
using System.IO;
using System.IO.Compression;
using Compression.Data;
using NirvanaCommon;
using VariantGrouping;
using Version1.Data;
using Version1.IO;

namespace CreateGnomadVersion1
{
    static class Program
    {
        static void Main(string [] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine($"USAGE: {Path.GetFileName(Environment.GetCommandLineArgs()[0])} <SA directory> <common threshold>");
                Environment.Exit(1);
            }

            string saDir       = args[0];
            string commonThreshold = args[1];

            string commonTsvPath = Path.Combine(saDir, $"gnomAD_chr1_common_{commonThreshold}.tsv.gz");
            string rareTsvPath   = Path.Combine(saDir, $"gnomAD_chr1_rare_{commonThreshold}.tsv.gz");
            string saPath        = Path.Combine(saDir, $"gnomad_chr1_v1_{commonThreshold}.nsa");
            string indexPath     = saPath + ".idx";
            string dictPath      = Path.Combine(saDir, "gnomad.dict");
            
            byte[] dictionaryBytes = File.ReadAllBytes(dictPath);
            var    dict            = new ZstdDictionary(CompressionMode.Compress, dictionaryBytes, 17);

            ChromosomeIndex[] chromosomeIndices;
            var               benchmark = new Benchmark();

            using (FileStream saStream = FileUtilities.GetWriteStream(saPath))
            using (var writer = new AlleleFrequencyWriter(saStream, GRCh37.Assembly, GnomAD.DataSourceVersion,
                GnomAD.JsonKey, dictionaryBytes, GRCh37.NumRefSeqs))
            {
                var bitArray = new BitArray(GRCh37.Chr1.Length);

                Console.WriteLine("- creating common blocks:");
                var commonBenchmark = new Benchmark();
                writer.StartCommon();
                // bit array is only used for rare variants
                CompressPipeline.RunPipeline(commonTsvPath, SaConstants.MaxCommonEntries, dict, writer, null).Wait();
                writer.EndCommon();
                ShowElapsedTime(commonBenchmark);

                Console.WriteLine("- creating rare blocks:");
                var rareBenchmark = new Benchmark();
                writer.StartRare();
                CompressPipeline.RunPipeline(rareTsvPath, SaConstants.MaxRareEntries, dict, writer, bitArray).Wait();
                writer.EndRare();
                ShowElapsedTime(rareBenchmark);

                writer.EndChromosome(GRCh37.Chr1, bitArray);
                chromosomeIndices = writer.ChromosomeIndices;
            }

            var indexBenchmark = new Benchmark();
            Console.WriteLine("- creating index:");

            using (FileStream idxStream = FileUtilities.GetWriteStream(indexPath))
            using (var writer = new IndexWriter(idxStream, GRCh37.NumRefSeqs))
            {
                var context = new ZstdContext(CompressionMode.Compress);
                writer.Write(chromosomeIndices, context);
            }

            ShowElapsedTime(indexBenchmark);
            Console.WriteLine($"- total time: {benchmark.GetElapsedTime()}");
        }

        private static void ShowElapsedTime(Benchmark benchmark)
        {
            Console.WriteLine($"  - elapsed time: {benchmark.GetElapsedTime()}");
            Console.WriteLine($"  - current RAM:  {MemoryUtilities.GetCurrentMemoryUsage()}");
            Console.WriteLine($"  - peak RAM:     {MemoryUtilities.GetPeakMemoryUsage()}\n");
        }
    }
}