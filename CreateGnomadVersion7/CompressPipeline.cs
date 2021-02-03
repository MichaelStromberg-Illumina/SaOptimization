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
using Version7.Data;
using Version7.IO;
using Version7.Utilities;

namespace CreateGnomadVersion7
{
    public static class CompressPipeline
    {
        public static async Task RunPipeline(string tsvPath, int maxBlockSize,
            AlleleFrequencyWriter writer)
        {
            var               context = new ThreadLocal<ZstdContext>(() => new ZstdContext(CompressionMode.Compress));
            var               cts     = new CancellationTokenSource();
            CancellationToken token   = cts.Token;

            ChannelReader<ConvertedData> byteGen = GetByteArrays(tsvPath, maxBlockSize, cts);
            ChannelReader<WriteBlock> compressedBlocks =
                CompressByteArrays(Split(byteGen, Environment.ProcessorCount, 2), context, token);

            int numBlocksWritten = await SortAndWriteBlocks(writer, compressedBlocks, token);

            if (token.IsCancellationRequested)
            {
                Console.WriteLine("ERROR: Cancel token enabled. RunPipeline aborted.");
                Environment.Exit(1);
            }

            Console.WriteLine($"  - compress pipeline: {numBlocksWritten:N0} blocks");
            context.Dispose();
        }

        private static async Task<int> SortAndWriteBlocks(AlleleFrequencyWriter writer, ChannelReader<WriteBlock> compressedBlocks, CancellationToken cancelToken)
        {
            var currentIndex = 0;
            var comparer     = new BlockComparer();
            var heap         = new MinHeap<WriteBlock>(comparer.Compare);

            await foreach (WriteBlock block in compressedBlocks.ReadAllAsync(cancelToken))
            {
                if (cancelToken.IsCancellationRequested) return 0;
                
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

        private static ChannelReader<ConvertedData> GetByteArrays(string tsvPath, int maxBlockSize, CancellationTokenSource cts)
        {
            var               output = Channel.CreateBounded<ConvertedData>(10);
            CancellationToken token  = cts.Token;

            Task.Run(async () =>
            {
                string line = null;
                
                try
                {
                    var lastPosition = 0;
                    var entries      = new List<SaEntry>(maxBlockSize);
                    var index        = 0;

                    using (var reader = new StreamReader(new GZipStream(FileUtilities.GetReadStream(tsvPath),
                        CompressionMode.Decompress)))
                    {
                        while (true)
                        {
                            line = await reader.ReadLineAsync();
                            if (string.IsNullOrEmpty(line)) break;

                            SaEntry entry = JsonUtilities.Deserialize(line);
                        
                            if (entries.Count >= maxBlockSize && lastPosition != entry.Position)
                            {
                                await output.Writer.WriteAsync(GetBytes(entries, index++), token);
                                entries.Clear();
                            }

                            entries.Add(entry);
                            lastPosition = entry.Position;
                        }

                        if (entries.Count > 0)
                        {
                            await output.Writer.WriteAsync(GetBytes(entries, index), token);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"ERROR: {e.Message}\nline:{line}");
                    cts.Cancel();
                }

                output.Writer.Complete();
            }, token);

            return output;
        }

        private static ConvertedData GetBytes(List<SaEntry> entries, int index)
        {
            byte[] bytes;

            using (var stream = new MemoryStream())
            using (var writer = new ExtendedBinaryWriter(stream))
            {
                writer.Write(entries.Count);
                foreach (SaEntry entry in entries) entry.Write(writer);
                bytes = stream.ToArray();
            }

            return new ConvertedData(entries[^1].Position, bytes, index);
        }

        private static ChannelReader<WriteBlock> CompressByteArrays(ChannelReader<ConvertedData>[] inputs,
            ThreadLocal<ZstdContext> context, CancellationToken token)
        {
            var output = Channel.CreateUnbounded<WriteBlock>();

            Task.Run(async () =>
            {
                async Task Redirect(ChannelReader<ConvertedData> input)
                {
                    await foreach (ConvertedData data in input.ReadAllAsync(token))
                    {
                        int numDataBytes = data.Bytes.Length;
                        
                        int compressedBufferSize = ZstandardStatic.GetCompressedBufferBounds(numDataBytes);
                        var compressedBytes      = new byte[compressedBufferSize];
                        
                        int numCompressedBytes = ZstandardStatic.Compress(data.Bytes, numDataBytes,
                            compressedBytes, compressedBufferSize, context.Value);

                        // double percentOriginal = numCompressedBytes / (double)numDataBytes * 100.0;
                        // Console.WriteLine($"- # compressed bytes: {numCompressedBytes:N0}, # original bytes: {numDataBytes} ({percentOriginal:0.0}%)");

                        var block = new WriteBlock(compressedBytes, numCompressedBytes, numDataBytes, data.LastPosition,
                            data.Index);
                        await output.Writer.WriteAsync(block, token);
                    }
                }
        
                await Task.WhenAll(inputs.Select(Redirect).ToArray());
                output.Writer.Complete();
            }, token);

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