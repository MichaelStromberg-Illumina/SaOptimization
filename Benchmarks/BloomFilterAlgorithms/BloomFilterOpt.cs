using System;
using System.Collections;
using System.Runtime.CompilerServices;
using NirvanaCommon;

namespace Benchmarks.BloomFilterAlgorithms
{
	public class BloomFilterOpt
	{
		private readonly BitArray _hashBits;
        private readonly int _m;
        private readonly int _k;
        private readonly ulong _seed;

        public BloomFilterOpt(int capacity, double errorRate) : this(capacity, BestM(capacity, errorRate),
            BestK(capacity, errorRate))
        {
        }
        
        public BloomFilterOpt(int capacity, int m, int k)
		{
            if (capacity < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "capacity must be > 0");
			}

			if (m < 1)
			{
				throw new ArgumentOutOfRangeException(
                    $"The provided capacity and errorRate values would result in an array of length > int.MaxValue. Please reduce either of these values. Capacity: {capacity}");
			}

            _m = m;
            _k = k;
			_hashBits = new BitArray(m);
            _seed = Seed.SplitMix64(1);
        }

        public void Add(ulong key)
        {
            ulong hash = MurmurFinalizer(key + _seed);
            ulong a    = (hash >> 32) | (hash << 32);
            ulong b    = hash;

            for (int i = 0; i < _k; i++)
            {
                int bitPos = FastModulo((uint) (a >> 32), _m);
                _hashBits.Set(bitPos, true);
                a += b;
            }
        }

		public bool Contains(ulong key)
		{
            ulong hash = MurmurFinalizer(key + _seed);
            ulong a    = (hash >> 32) | (hash << 32);
            ulong b    = hash;
            
            for (int i = 0; i < _k; i++)
			{
                int bitPos = FastModulo((uint) (a >> 32), _m);
				if (!_hashBits.Get(bitPos)) return false;
                a += b;
			}

			return true;
		}

		public static int BestK(int capacity, double errorRate) => (int)Math.Round(Math.Log(2.0) * BestM(capacity, errorRate) / capacity);

        public static int BestM(int capacity, double errorRate) => (int)Math.Ceiling(capacity * Math.Log(errorRate, 1.0 / Math.Pow(2, Math.Log(2.0))));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FastModulo(uint hash, int max) => (int) ((hash * (ulong)max) >> 32);
        
        private const ulong C1 = 0xff51afd7ed558ccd;
        private const ulong C2 = 0xc4ceb9fe1a85ec53;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong MurmurFinalizer(ulong h)
        {
            h ^= h >> 33;
            h *= C1;
            h ^= h >> 33;
            h *= C2;
            h ^= h >> 33;
            return h;
        }
	}
}