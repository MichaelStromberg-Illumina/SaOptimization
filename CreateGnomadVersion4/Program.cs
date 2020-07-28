﻿using System;
using System.IO;
using System.IO.Compression;
using Compression.Data;
using NirvanaCommon;
using VariantGrouping;
using Version4.Data;
using Version4.IO;

namespace CreateGnomadVersion4
{
    static class Program
    {
        static void Main()
        {
            byte[] dictionaryBytes = File.ReadAllBytes(GnomAD.DictionaryPath);
            var    dict            = new ZstdDictionary(CompressionMode.Compress, dictionaryBytes, 17);

            ChromosomeIndex[] chromosomeIndices;
            var               benchmark = new Benchmark();

            using (FileStream saStream = FileUtilities.GetWriteStream(SaConstants.SaPath))
            using (var writer = new AlleleFrequencyWriter(saStream, GRCh37.Assembly, GnomAD.DataSourceVersion,
                GnomAD.JsonKey, dictionaryBytes, GRCh37.NumRefSeqs))
            {
                var bitArray = new BitArray(GRCh37.Chr1.Length);

                Console.WriteLine("- creating common blocks:");
                var commonBenchmark = new Benchmark();
                writer.StartCommon();
                // bit array is only used for rare variants
                CompressPipeline.RunPipeline(Pedigree.CommonTsvPath, SaConstants.MaxCommonEntries, dict, writer, null).Wait();
                writer.EndCommon();
                ShowElapsedTime(commonBenchmark);

                Console.WriteLine("- creating rare blocks:");
                var rareBenchmark = new Benchmark();
                writer.StartRare();
                CompressPipeline.RunPipeline(Pedigree.RareTsvPath, SaConstants.MaxRareEntries, dict, writer, bitArray).Wait();
                writer.EndRare();
                ShowElapsedTime(rareBenchmark);

                writer.EndChromosome(GRCh37.Chr1, bitArray);
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