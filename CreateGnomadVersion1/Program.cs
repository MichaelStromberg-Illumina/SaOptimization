using System;
using System.IO;
using System.IO.Compression;
using Compression.Data;
using NirvanaCommon;
using Version1.Data;
using Version1.IO;

namespace CreateGnomadVersion1
{
    static class Program
    {
        private static void Main()
        {
            const string commonPath = @"E:\Data\Nirvana\NewSA\gnomAD_chr1_common.tsv.gz";
            const string rarePath   = @"E:\Data\Nirvana\NewSA\gnomAD_chr1_rare.tsv.gz";
            const string saPath     = @"E:\Data\Nirvana\NewSA\gnomad_chr1.nsa";
            const string indexPath  = saPath + ".idx";

            const GenomeAssembly genomeAssembly  = GenomeAssembly.GRCh37;
            DateTime             releaseDate     = DateTime.Parse("2018-10-17");
            byte[]               dictionaryBytes = File.ReadAllBytes(@"E:\Data\Nirvana\NewSA\gnomad.dict");
            const int            numRefSeqs      = 1;

            var dict = new ZstdDictionary(CompressionMode.Compress, dictionaryBytes, 17);
            var chr1 = new Chromosome("chr1", "1", null, null, 249250621, 0);
            
            var dataSourceVersion = new DataSourceVersion("gnomAD", "2.1", releaseDate,
                "Allele frequencies from Genome Aggregation Database (gnomAD)");

            ChromosomeIndex[] chromosomeIndices;
            var benchmark = new Benchmark();
            
            using (FileStream saStream = FileUtilities.GetWriteStream(saPath))
            using (var writer = new AlleleFrequencyWriter(saStream, genomeAssembly, dataSourceVersion,
                "gnomad", dictionaryBytes, numRefSeqs))
            {
                var bitArray = new BitArray(chr1.Length);
                
                Console.WriteLine("- creating common blocks:");
                var commonBenchmark = new Benchmark();
                writer.StartCommon();
                // bit array is only used for rare variants
                CompressPipeline.RunPipeline(commonPath, SaConstants.MaxCommonEntries, dict, writer, null).Wait();
                writer.EndCommon();
                ShowElapsedTime(commonBenchmark);

                Console.WriteLine("- creating rare blocks:");
                var rareBenchmark = new Benchmark();
                writer.StartRare();
                CompressPipeline.RunPipeline(rarePath, SaConstants.MaxRareEntries, dict, writer, bitArray).Wait();
                writer.EndRare();
                ShowElapsedTime(rareBenchmark);

                writer.EndChromosome(chr1, bitArray);
                chromosomeIndices = writer.ChromosomeIndices;
            }
            
            var indexBenchmark = new Benchmark();
            Console.WriteLine("- creating index:");
            
            using (FileStream idxStream = FileUtilities.GetWriteStream(indexPath))
            using (var writer = new IndexWriter(idxStream, numRefSeqs))
            {
                writer.Write(chromosomeIndices);
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