using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks.Dataflow;
using Compression.Algorithms;
using Version1.Data;
using Version1.IO;
using Version1.Nirvana;
using Version1.Utilities;

namespace CreateGnomadVersion1
{
    class Program
    {
        static void Main()
        {
            const string commonPath = @"E:\Data\Nirvana\gnomAD_chr1_common.tsv.gz";
            const string rarePath   = @"E:\Data\Nirvana\gnomAD_chr1_rare.tsv.gz";
            const string saPath     = @"E:\Data\Nirvana\NewSA\gnomad_chr1.nsa";
            const string indexPath  = saPath + ".idx";

            var       genomeAssembly  = GenomeAssembly.GRCh37;
            DateTime  releaseDate     = DateTime.Parse("2018-10-17");
            byte[]    dictionaryBytes = File.ReadAllBytes(@"E:\Data\Nirvana\NewSA\gnomad.dict");
            const int numRefSeqs      = 1;

            
            var chr1 = new Chromosome("chr1", "1", null, null, -1, 0);
            
            var dataSourceVersion = new DataSourceVersion("gnomAD", "2.1", releaseDate,
                "Allele frequencies from Genome Aggregation Database (gnomAD)");
            
            using (FileStream saStream    = FileUtilities.GetWriteStream(saPath))
            using (FileStream indexStream = FileUtilities.GetWriteStream(indexPath))
            using (var writer = new AlleleFrequencyWriter(saStream, indexStream, genomeAssembly, dataSourceVersion,
                "gnomad", dictionaryBytes, numRefSeqs))
            {
                var entries = new List<TsvEntry>(SaConstants.MaxCommonEntries);
                var blocks  = new List<Block>();

                Console.Write("- creating common blocks... ");
                var commonBenchmark = new Benchmark();
                CreateBlocks(commonPath, dictionaryBytes, entries, blocks, SaConstants.MaxCommonEntries);
                Console.WriteLine($"  - {blocks.Count} blocks created ({commonBenchmark.GetElapsedTime()})");
                
                Console.Write("- writing common blocks... ");
                var commonWriteBenchmark = new Benchmark();
                writer.WriteBlocks(chr1, blocks);
                Console.WriteLine(commonWriteBenchmark.GetElapsedTime());
                
                Console.Write("- creating rare blocks...");
                var rareBenchmark = new Benchmark();
                CreateBlocks(rarePath, dictionaryBytes, entries, blocks, SaConstants.MaxRareEntries);
                Console.WriteLine($"  - {blocks.Count} blocks created ({rareBenchmark.GetElapsedTime()})");
                
                Console.Write("- writing rare blocks... ");
                var rareWriteBenchmark = new Benchmark();
                writer.WriteBlocks(chr1, blocks);
                Console.WriteLine(rareWriteBenchmark.GetElapsedTime());
            }
        }
        
        private static ConvertedData GetBytes(List<TsvEntry> entries)
        {
            Console.WriteLine("Getting bytes");
            byte[] bytes;

            var lastPosition = 0;

            using (var stream = new MemoryStream())
            using (var writer = new ExtendedBinaryWriter(stream))
            {
                foreach (TsvEntry entry in entries)
                {
                    VariantType variantType = VariantTypeUtils.GetVariantType(entry.RefAllele, entry.AltAllele);
                    VariantTypeUtils.CheckVariantType(variantType);

                    string allele        = variantType == VariantType.deletion ? entry.RefAllele : entry.AltAllele;
                    int    deltaPosition = entry.Position - lastPosition;

                    writer.Write((byte) variantType);
                    writer.WriteOpt(deltaPosition);
                    writer.Write(allele);
                    writer.Write(entry.Json);

                    lastPosition = entry.Position;
                }

                bytes = stream.ToArray();
            }

            return new ConvertedData(lastPosition, entries.Count, bytes);
        }
        
        private static Block CompressBytes(ConvertedData data, byte[] dictionaryBytes)
        {
            Console.WriteLine("Compressing bytes");
            int numDataBytes = data.Bytes.Length;
            
            var zstd = new ZstandardDict(17, dictionaryBytes);
            int compressedBufferSize = zstd.GetCompressedBufferBounds(numDataBytes);
            var compressedBytes = new byte[compressedBufferSize];
            
            int numCompressedBytes = zstd.Compress(data.Bytes, numDataBytes, 
                compressedBytes, compressedBufferSize);
            
            return new Block(data.LastPosition, data.NumEntries, compressedBytes, numCompressedBytes);
        }

        private static void CreateBlocks(string tsvPath, byte[] dictionaryBytes, List<TsvEntry> entries, List<Block> blocks, int maxBlockSize)
        {
            // add stuff to convert block here
            var lastPosition = 0;
            entries.Clear();
            blocks.Clear();
            
            var compressBlock = new TransformBlock<ConvertedData, Block>(
                convertedData => CompressBytes(convertedData, dictionaryBytes),
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 10,
                    BoundedCapacity        = 10
                });

            var addBlock = new ActionBlock<Block>(blocks.Add);

            var linkOptions = new DataflowLinkOptions {PropagateCompletion = true};
            // convertBlock.LinkTo(compressBlock, linkOptions);
            compressBlock.LinkTo(addBlock, linkOptions);
                
            using (var reader = new StreamReader(new GZipStream(FileUtilities.GetReadStream(tsvPath),
                CompressionMode.Decompress)))
            {
                while (true)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line)) break;
                    
                    TsvEntry entry = TsvEntryUtils.GetTsvEntry(line);
                    if (entries.Count >= maxBlockSize && lastPosition != entry.Position)
                    {
                        // compressBlock.Post(GetBytes(entries));
                        compressBlock.SendAsync(GetBytes(entries)).ConfigureAwait(false).GetAwaiter().GetResult();
                        // blocks.Add(new Block(zstd, entries));
                        entries.Clear();
                    }
                        
                    entries.Add(entry);
                    lastPosition = entry.Position;
                }
            
                if (entries.Count > 0)
                {
                    // compressBlock.Post(GetBytes(entries));
                    compressBlock.SendAsync(GetBytes(entries)).ConfigureAwait(false).GetAwaiter().GetResult();
                    // blocks.Add(new Block(zstd, entries));
                    entries.Clear();
                }
            }
            
            compressBlock.Complete();
            addBlock.Completion.Wait();
        }
    }
}