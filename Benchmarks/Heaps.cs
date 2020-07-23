using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Benchmarks.HeapAlgorithms;
using Version1.Data;
using Version1.Utilities;

namespace Benchmarks
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    [MemoryDiagnoser]
    public class Heaps
    {
        private readonly WriteBlock[]          _shuffledBlocks;
        private readonly IComparer<WriteBlock> _comparer = new BlockComparer();

        public Heaps() => _shuffledBlocks = GetShuffledBlocks(1000);

        private static WriteBlock[] GetShuffledBlocks(int numBlocks)
        {
            var blocks = new WriteBlock[numBlocks];
            for (var i = 0; i < numBlocks; i++) blocks[i] = new WriteBlock(null, -1, -1, -1, i);
            SortUtils.Shuffle(blocks);
            return blocks;
        }
        
        [Benchmark(Baseline = true)]
        public int Current()
        {
            int currentIndex = 0;
            var heap         = new MinHeap_Current<WriteBlock>(_comparer.Compare);

            foreach (WriteBlock block in _shuffledBlocks)
            {
                heap.Add(block);
                while (heap.Count() > 0 && currentIndex == heap.GetMin().Index)
                {
                    heap.ExtractMin();
                    currentIndex++;
                }
            }

            while (heap.Count() > 0) heap.ExtractMin();

            return currentIndex;
        }

        [Benchmark]
        public int Heap2()
        {
            int currentIndex = 0;
            var heap         = new MinHeap2<WriteBlock>(_comparer.Compare);
        
            foreach (WriteBlock block in _shuffledBlocks)
            {
                heap.Add(block);
                while (heap.Count > 0 && currentIndex == heap.Minimum.Index)
                {
                    heap.RemoveMin();
                    currentIndex++;
                }
            }
        
            while (heap.Count > 0) heap.RemoveMin();
        
            return currentIndex;
        }

        [Benchmark]
        public int Heap3()
        {
            int currentIndex = 0;
            var heap         = new MinHeap3<WriteBlock>(_comparer.Compare);

            foreach (WriteBlock block in _shuffledBlocks)
            {
                heap.Add(block);
                while (heap.Count > 0 && currentIndex == heap.Minimum.Index)
                {
                    heap.RemoveMin();
                    currentIndex++;
                }
            }

            while (heap.Count > 0) heap.RemoveMin();

            return currentIndex;
        }

        [Benchmark]
        public int LR_Localized()
        {
            int currentIndex = 0;
            var heap         = new MinHeap_LR_Localized<WriteBlock>(_comparer.Compare);

            foreach (WriteBlock block in _shuffledBlocks)
            {
                heap.Add(block);
                while (heap.Count > 0 && currentIndex == heap.Minimum.Index)
                {
                    heap.RemoveMin();
                    currentIndex++;
                }
            }

            while (heap.Count > 0) heap.RemoveMin();

            return currentIndex;
        }
        
        [Benchmark]
        public int P_Localized()
        {
            int currentIndex = 0;
            var heap         = new MinHeap_P_Localized<WriteBlock>(_comparer.Compare);

            foreach (WriteBlock block in _shuffledBlocks)
            {
                heap.Add(block);
                while (heap.Count > 0 && currentIndex == heap.Minimum.Index)
                {
                    heap.RemoveMin();
                    currentIndex++;
                }
            }

            while (heap.Count > 0) heap.RemoveMin();

            return currentIndex;
        }
        
        [Benchmark]
        public int All_Localized()
        {
            int currentIndex = 0;
            var heap         = new MinHeap_All_Localized<WriteBlock>(_comparer.Compare);

            foreach (WriteBlock block in _shuffledBlocks)
            {
                heap.Add(block);
                while (heap.Count > 0 && currentIndex == heap.Minimum.Index)
                {
                    heap.RemoveMin();
                    currentIndex++;
                }
            }

            while (heap.Count > 0) heap.RemoveMin();

            return currentIndex;
        }
        
        [Benchmark]
        public int Array()
        {
            int currentIndex = 0;
            var heap         = new MinHeap_All_Array<WriteBlock>(_comparer.Compare);

            foreach (WriteBlock block in _shuffledBlocks)
            {
                heap.Add(block);
                while (heap.Count > 0 && currentIndex == heap.Minimum.Index)
                {
                    heap.RemoveMin();
                    currentIndex++;
                }
            }

            while (heap.Count > 0) heap.RemoveMin();

            return currentIndex;
        }
        
        [Benchmark]
        public int SortedList()
        {
            int currentIndex = 0;
            var list         = new SortedList<int, WriteBlock>();
        
            foreach (WriteBlock block in _shuffledBlocks)
            {
                list.Add(block.Index, block);
        
                while (list.Count > 0 && currentIndex == list.Keys[0])
                {
                    list.RemoveAt(0);
                    currentIndex++;
                }
            }
        
            while (list.Count > 0)
            {
                list.RemoveAt(0);
            }
        
            return currentIndex;
        }
    }
}