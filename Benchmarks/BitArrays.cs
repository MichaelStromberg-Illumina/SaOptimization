using System;
using System.Collections;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Benchmarks.BitArrayAlgorithms;

namespace Benchmarks
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    [MemoryDiagnoser]
    public class BitArrays
    {
        private readonly int[] _positions;

        private const int NumPositions = 10000;
        private const int MaxPosition  = 100000;

        public BitArrays() => _positions = GetRandomPositions(NumPositions, MaxPosition);

        private static int[] GetRandomPositions(int numPositions, int maxPosition)
        {
            var random    = new Random();
            var positions = new int[numPositions];

            for (var i = 0; i < numPositions; i++) positions[i] = random.Next(1, maxPosition);
            return positions;
        }

        [Benchmark(Baseline = true)]
        public int BitArray()
        {
            int numSet = 0;
            var bitArray = new BitArray(MaxPosition);
            foreach (int position in _positions)
            {
                bitArray.Set(position, true);
                if (bitArray[position]) numSet++;
            }

            if (numSet != NumPositions) throw new InvalidDataException();
            return numSet;
        }
        
        // [Benchmark]
        // public int BitArray1()
        // {
        //     int numSet = 0;
        //     var bitArray = new BitArray1(MaxPosition);
        //     
        //     foreach (int position in _positions)
        //     {
        //         bitArray.Set(position);
        //         if(bitArray.Get(position)) numSet++;
        //     }
        //     
        //     if (numSet != NumPositions) throw new InvalidDataException();
        //     return numSet;
        // }
        
        [Benchmark]
        public int BitArray_Inline()
        {
            int numSet   = 0;
            var bitArray = new BitArray_Inline(MaxPosition);
            
            foreach (int position in _positions)
            {
                bitArray.Set(position);
                if(bitArray.Get(position)) numSet++;
            }
            
            if (numSet != NumPositions) throw new InvalidDataException();
            return numSet;
        }

        [Benchmark]
        public int ChromosomeBitMap()
        {
            int numSet           = 0;
            var chromosomeBitMap = new ChromosomeBitMap(MaxPosition);

            foreach (int position in _positions)
            {
                chromosomeBitMap.Set(position);
                if (chromosomeBitMap.IsSet(position)) numSet++;
            }

            if (numSet != NumPositions) throw new InvalidDataException();
            return numSet;
        }
    }
}