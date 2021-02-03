using System;
using System.IO;
using System.IO.Compression;
using Compression.Data;
using NirvanaCommon;
using VariantGrouping;
using Version2.Data;
using Version2.IO;
using Version2.Utilities;

namespace CreateGnomadVersion2
{
    internal static class Program
    {
        private static void Main()
        {
            byte[] dictionaryBytes = File.ReadAllBytes(GnomAD.DictionaryPath);
            var    dict            = new ZstdDictionary(CompressionMode.Compress, dictionaryBytes, 17);

            (string saPath, string indexPath) = SaPath.GetPaths(SupplementaryAnnotation.Directory);
            
            ChromosomeIndex[] chromosomeIndices;
            var               benchmark = new Benchmark();

            using (FileStream saStream = FileUtilities.GetWriteStream(saPath))
            using (var writer = new AlleleFrequencyWriter(saStream, GRCh37.Assembly, GnomAD.DataSourceVersion,
                GnomAD.JsonKey, dictionaryBytes, GRCh37.NumRefSeqs))
            {
                var commonBitArray = new BitArray(GRCh37.Chr1.Length);
                var rareBitArray   = new BitArray(GRCh37.Chr1.Length);
                
                Console.WriteLine("- creating common blocks:");
                var commonBenchmark = new Benchmark();
                writer.StartCommon();
                CompressPipeline.RunPipeline(Pedigree.CommonTsvPath, SaConstants.MaxCommonEntries, dict, writer, commonBitArray).Wait();
                writer.EndCommon();
                ShowElapsedTime(commonBenchmark);

                Console.WriteLine("- creating rare blocks:");
                var rareBenchmark = new Benchmark();
                writer.StartRare();
                CompressPipeline.RunPipeline(Pedigree.RareTsvPath, SaConstants.MaxRareEntries, dict, writer, rareBitArray).Wait();
                writer.EndRare();
                ShowElapsedTime(rareBenchmark);

                writer.EndChromosome(GRCh37.Chr1, commonBitArray, rareBitArray);
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