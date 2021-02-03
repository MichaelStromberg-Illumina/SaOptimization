using System;
using System.IO;
using System.IO.Compression;
using Compression.Data;
using NirvanaCommon;
using VariantGrouping;
using Version6.Data;
using Version6.IO;
using Version6.Utilities;

namespace CreateGnomadVersion6
{
    internal static class Program
    {
        private static void Main(string [] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine($"USAGE: {Path.GetFileName(Environment.GetCommandLineArgs()[0])} <SA directory> <common threshold> <common block size> <rare block size>");
                Environment.Exit(1);
            }

            string saDir           = args[0];
            string commonThreshold = args[1];
            int    commonBlockSize = int.Parse(args[2]);
            int    rareBlockSize   = int.Parse(args[3]);
            
            string commonTsvPath = Path.Combine(saDir, $"gnomAD_chr1_common_{commonThreshold}.tsv.gz");
            string rareTsvPath   = Path.Combine(saDir, $"gnomAD_chr1_rare_{commonThreshold}.tsv.gz");

            (string saPath, string indexPath) = SaPath.GetPaths(saDir, commonThreshold, commonBlockSize, rareBlockSize);
            
            byte[] dictionaryBytes = File.ReadAllBytes(GnomAD.DictionaryPath);
            var    dict            = new ZstdDictionary(CompressionMode.Compress, dictionaryBytes, 17);

            ChromosomeIndex[] chromosomeIndices;
            var               benchmark = new Benchmark();

            using (FileStream saStream = FileUtilities.GetWriteStream(saPath))
            using (var writer = new AlleleFrequencyWriter(saStream, GRCh37.Assembly, GnomAD.DataSourceVersion,
                GnomAD.JsonKey, dictionaryBytes, GRCh37.NumRefSeqs))
            {
                Console.WriteLine("- create allele index:");
                var alleleIndexBenchmark = new Benchmark();
                (ulong[] positionAlleles, ulong[] commonPositionAlleles) = AlleleIndex.GetAllelesAsync(commonTsvPath, rareTsvPath).Result;
                Console.WriteLine($"  - {positionAlleles.Length:N0} position alleles, {commonPositionAlleles.Length:N0} common position alleles found");
                ShowElapsedTime(alleleIndexBenchmark);

                Console.WriteLine("- create large XOR filter:");
                var xorBenchmark = new Benchmark();
                var xorFilter    = Xor8.Construction(positionAlleles);
                Console.WriteLine($"  - size: {xorFilter.Data.Length:N0} bytes");
                ShowElapsedTime(xorBenchmark);
                
                Console.WriteLine("- creating common blocks:");
                var commonBenchmark = new Benchmark();
                writer.StartCommon();
                CompressPipeline.RunPipeline(commonTsvPath, commonBlockSize, dict, writer).Wait();
                writer.EndCommon();
                ShowElapsedTime(commonBenchmark);

                Console.WriteLine("- creating rare blocks:");
                var rareBenchmark = new Benchmark();
                writer.StartRare();
                CompressPipeline.RunPipeline(rareTsvPath, rareBlockSize, dict, writer).Wait();
                writer.EndRare();
                ShowElapsedTime(rareBenchmark);

                writer.EndChromosome(GRCh37.Chr1, xorFilter, commonPositionAlleles);
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