using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Compression.Data;
using NirvanaCommon;
using VariantGrouping;
using Version5.Data;
using Version5.IO;

namespace CreateGnomadVersion5
{
    internal static class Program
    {
        private static void Main()
        {
            byte[] dictionaryBytes = File.ReadAllBytes(GnomAD.DictionaryPath);
            var    dict            = new ZstdDictionary(CompressionMode.Compress, dictionaryBytes, 17);

            ChromosomeIndex[] chromosomeIndices;
            var               benchmark = new Benchmark();

            using (FileStream saStream = FileUtilities.GetWriteStream(SaConstants.SaPath))
            using (var writer = new AlleleFrequencyWriter(saStream, GRCh37.Assembly, GnomAD.DataSourceVersion,
                GnomAD.JsonKey, dictionaryBytes, GRCh37.NumRefSeqs))
            {
                Console.WriteLine("- create allele index:");
                var alleleIndexBenchmark = new Benchmark();
                (WriteBlock alleleBlock, Dictionary<string, int> alleleToIndex, ulong[] positionAlleles,
                        ulong[] commonPositionAlleles) =
                    AlleleIndex.GetAllelesAsync(Pedigree.CommonTsvPath, Pedigree.RareTsvPath).Result;
                Console.WriteLine($"  - {alleleToIndex.Count:N0} alleles, {positionAlleles.Length:N0} pa, {commonPositionAlleles.Length:N0} common pa found");
                writer.WriteAlleles(alleleBlock);
                ShowElapsedTime(alleleIndexBenchmark);

                Console.WriteLine("- create large XOR filter:");
                var xorBenchmark = new Benchmark();
                var xorFilter    = Xor8.Construction(positionAlleles);
                Console.WriteLine($"  - size: {xorFilter.Data.Length:N0} bytes");
                ShowElapsedTime(xorBenchmark);
                
                Console.WriteLine("- creating common blocks:");
                var commonBenchmark = new Benchmark();
                writer.StartCommon();
                CompressPipeline.RunPipeline(Pedigree.CommonTsvPath, SaConstants.MaxCommonEntries, dict, writer, alleleToIndex).Wait();
                writer.EndCommon();
                ShowElapsedTime(commonBenchmark);

                Console.WriteLine("- creating rare blocks:");
                var rareBenchmark = new Benchmark();
                writer.StartRare();
                CompressPipeline.RunPipeline(Pedigree.RareTsvPath, SaConstants.MaxRareEntries, dict, writer, alleleToIndex).Wait();
                writer.EndRare();
                ShowElapsedTime(rareBenchmark);

                writer.EndChromosome(GRCh37.Chr1, xorFilter, commonPositionAlleles);
                chromosomeIndices = writer.ChromosomeIndices;
            }

            var indexBenchmark = new Benchmark();
            Console.WriteLine("- creating index:");

            using (FileStream idxStream = FileUtilities.GetWriteStream(SaConstants.IndexPath))
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