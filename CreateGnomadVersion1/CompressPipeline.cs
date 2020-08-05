using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Compression.Algorithms;
using Compression.Data;
using NirvanaCommon;
using VariantGrouping;
using Version1.Data;
using Version1.IO;
using Version1.Utilities;

namespace CreateGnomadVersion1
{
    public static class CompressPipeline
    {
        public static async Task RunPipeline(string tsvPath, int maxBlockSize, ZstdDictionary dict,
            AlleleFrequencyWriter writer, BitArray bitArray)
        {
            var context = new ThreadLocal<ZstdContext>(() => new ZstdContext(CompressionMode.Compress));
            
            ChannelReader<ConvertedData> byteGen = GetByteArrays(tsvPath, maxBlockSize, bitArray);
            ChannelReader<WriteBlock> compressedBlocks =
                CompressByteArrays2(Split(byteGen, Environment.ProcessorCount, 2), context, dict);

            int numBlocksWritten = await SortAndWriteBlocks(writer, compressedBlocks);

            Console.WriteLine($"  - compress pipeline: {numBlocksWritten:N0} blocks");
            context.Dispose();
        }

        private static async Task<int> SortAndWriteBlocks(AlleleFrequencyWriter writer, ChannelReader<WriteBlock> compressedBlocks)
        {
            int currentIndex = 0;
            var comparer     = new BlockComparer();
            var heap         = new MinHeap<WriteBlock>(comparer.Compare);

            await foreach (WriteBlock block in compressedBlocks.ReadAllAsync())
            {
                heap.Add(block);
                while (heap.Count > 0 && currentIndex == heap.Minimum.Index)
                {
                    writer.WriteBlock(heap.RemoveMin());
                    currentIndex++;
                }
            }

            while (heap.Count > 0) writer.WriteBlock(heap.RemoveMin());
            return currentIndex;
        }

        private static ChannelReader<ConvertedData> GetByteArrays(string tsvPath, int maxBlockSize, BitArray bitArray)
        {
            var output = Channel.CreateBounded<ConvertedData>(10);

            Task.Run(async () =>
            {
                var lastPosition = 0;
                var entries      = new List<TsvEntry>(maxBlockSize);
                var index        = 0;

                using (var reader = new StreamReader(new GZipStream(FileUtilities.GetReadStream(tsvPath),
                    CompressionMode.Decompress)))
                {
                    while (true)
                    {
                        string line = await reader.ReadLineAsync();
                        if (string.IsNullOrEmpty(line)) break;

                        TsvEntry entry = TsvEntryUtils.GetTsvEntry(line);
                        bitArray?.Set(entry.Position);
                        
                        if (entries.Count >= maxBlockSize && lastPosition != entry.Position)
                        {
                            await output.Writer.WriteAsync(GetBytes(entries, index++));
                            entries.Clear();
                        }

                        entries.Add(entry);
                        lastPosition = entry.Position;
                    }

                    if (entries.Count > 0)
                    {
                        await output.Writer.WriteAsync(GetBytes(entries, index));
                    }
                }

                output.Writer.Complete();
            });

            return output;
        }

        private static ConvertedData GetBytes(List<TsvEntry> entries, int index)
        {
            byte[] bytes;

            var lastPosition = 0;

            using (var stream = new MemoryStream())
            using (var writer = new ExtendedBinaryWriter(stream))
            {
                writer.WriteOpt(entries.Count);
                
                foreach (TsvEntry entry in entries)
                {
                    VariantType variantType = VariantTypeUtilities.GetVariantType(entry.RefAllele, entry.AltAllele);
                    VariantTypeUtilities.CheckVariantType(variantType);

                    string allele        = variantType == VariantType.deletion ? entry.RefAllele : entry.AltAllele;
                    int    deltaPosition = entry.Position - lastPosition;
                    
                    writer.WriteOpt(deltaPosition);
                    writer.Write((byte) variantType);
                    writer.Write(allele);
                    writer.Write(entry.Json);

                    lastPosition = entry.Position;
                }

                bytes = stream.ToArray();
            }

            return new ConvertedData(lastPosition, bytes, index);
        }
        
        private static ChannelReader<WriteBlock> CompressByteArrays2(ChannelReader<ConvertedData>[] inputs, ThreadLocal<ZstdContext> context, ZstdDictionary dict)
        {
            var output = Channel.CreateUnbounded<WriteBlock>();

            Task.Run(async () =>
            {
                async Task Redirect(ChannelReader<ConvertedData> input)
                {
                    await foreach (ConvertedData data in input.ReadAllAsync())
                    {
                        int numDataBytes = data.Bytes.Length;
                        
                        int compressedBufferSize = ZstandardDict.GetCompressedBufferBounds(numDataBytes);
                        var compressedBytes      = new byte[compressedBufferSize];
                        
                        int numCompressedBytes = ZstandardDict.Compress(data.Bytes, numDataBytes,
                            compressedBytes, compressedBufferSize, context.Value, dict);

                        var block = new WriteBlock(compressedBytes, numCompressedBytes, numDataBytes, data.LastPosition,
                            data.Index);
                        await output.Writer.WriteAsync(block);
                    }
                }
        
                await Task.WhenAll(inputs.Select(Redirect).ToArray());
                output.Writer.Complete();
            });

            return output;
        }
        
        private static ChannelReader<ConvertedData>[] Split(ChannelReader<ConvertedData> input, int numConsumers, int capacity)
        {
            var outputs = new Channel<ConvertedData>[numConsumers];

            for (var i = 0; i < numConsumers; i++) outputs[i] = Channel.CreateBounded<ConvertedData>(capacity);

            Task.Run(async () =>
            {
                var index = 0;
                await foreach (ConvertedData item in input.ReadAllAsync())
                {
                    await outputs[index].Writer.WriteAsync(item);
                    index++;
                    if (index == numConsumers) index = 0;
                }

                foreach (Channel<ConvertedData> ch in outputs) ch.Writer.Complete();
            });

            return outputs.Select(ch => ch.Reader).ToArray();
        }
    }
}