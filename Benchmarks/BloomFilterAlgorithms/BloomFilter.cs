using System;
using System.Collections;

namespace Benchmarks.BloomFilterAlgorithms
{
    /// <summary>
	/// Bloom filter.
	/// </summary>
	public class BloomFilter
	{
		private readonly int _hashFunctionCount;
		private readonly BitArray _hashBits;

        /// <summary>
		/// Creates a new Bloom filter, using the optimal size for the underlying data structure based on the desired capacity and error rate, as well as the optimal number of hash functions.
		/// </summary>
		/// <param name="capacity">The anticipated number of items to be added to the filter. More than this number of items can be added, but the error rate will exceed what is expected.</param>
		/// <param name="errorRate">The accepable false-positive rate (e.g., 0.01F = 1%)</param>
		public BloomFilter(int capacity, double errorRate)
			: this(capacity, errorRate, BestM(capacity, errorRate), BestK(capacity, errorRate))
		{
		}

		/// <summary>
		/// Creates a new Bloom filter.
		/// </summary>
		/// <param name="capacity">The anticipated number of items to be added to the filter. More than this number of items can be added, but the error rate will exceed what is expected.</param>
		/// <param name="errorRate">The accepable false-positive rate (e.g., 0.01F = 1%)</param>
		/// <param name="m">The number of elements in the BitArray.</param>
		/// <param name="k">The number of hash functions to use.</param>
		public BloomFilter(int capacity, double errorRate, int m, int k)
		{
			// validate the params are in range
			if (capacity < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "capacity must be > 0");
			}

			if (errorRate >= 1 || errorRate <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(errorRate), errorRate,
                    $"errorRate must be between 0 and 1, exclusive. Was {errorRate}");
			}

			// from overflow in bestM calculation
			if (m < 1)
			{
				throw new ArgumentOutOfRangeException(
                    $"The provided capacity and errorRate values would result in an array of length > int.MaxValue. Please reduce either of these values. Capacity: {capacity}, Error rate: {errorRate}");
			}

			_hashFunctionCount = k;
			_hashBits = new BitArray(m);
		}

        /// <summary>
		/// Adds a new item to the filter. It cannot be removed.
		/// </summary>
		/// <param name="item">The item.</param>
		public void Add(ulong item)
		{
			// start flipping bits for each hash of item
			int primaryHash = item.GetHashCode();
			int secondaryHash = HashUInt64(item);
			for (int i = 0; i < _hashFunctionCount; i++)
			{
				int hash = ComputeHash(primaryHash, secondaryHash, i);
				_hashBits[hash] = true;
			}
		}

		/// <summary>
		/// Checks for the existance of the item in the filter for a given probability.
		/// </summary>
		/// <param name="item"> The item. </param>
		/// <returns> The <see cref="bool"/>. </returns>
		public bool Contains(ulong item)
		{
			int primaryHash = item.GetHashCode();
			int secondaryHash = HashUInt64(item);
			for (int i = 0; i < _hashFunctionCount; i++)
			{
				int hash = ComputeHash(primaryHash, secondaryHash, i);
				if (_hashBits[hash] == false)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// The best k.
		/// </summary>
		/// <param name="capacity"> The capacity. </param>
		/// <param name="errorRate"> The error rate. </param>
		/// <returns> The <see cref="int"/>. </returns>
		private static int BestK(int capacity, double errorRate)
		{
			return (int)Math.Round(Math.Log(2.0) * BestM(capacity, errorRate) / capacity);
		}

		/// <summary>
		/// The best m.
		/// </summary>
		/// <param name="capacity"> The capacity. </param>
		/// <param name="errorRate"> The error rate. </param>
		/// <returns> The <see cref="int"/>. </returns>
		private static int BestM(int capacity, double errorRate)
		{
			return (int)Math.Ceiling(capacity * Math.Log(errorRate, 1.0 / Math.Pow(2, Math.Log(2.0))));
		}

        public static int HashUInt64(ulong key)
        {
            unchecked
            {
                key = ~key + (key << 18); // key = (key << 18) - key - 1;
                key ^= key >> 31;
                key *= 21; // key = (key + (key << 2)) + (key << 4);
                key ^= key >> 11;
                key += key << 6;
                key ^= key >> 22;
                return (int) key;
            }
        }

        /// <summary>
		/// Performs Dillinger and Manolios double hashing. 
		/// </summary>
		/// <param name="primaryHash"> The primary hash. </param>
		/// <param name="secondaryHash"> The secondary hash. </param>
		/// <param name="i"> The i. </param>
		/// <returns> The <see cref="int"/>. </returns>
		private int ComputeHash(int primaryHash, int secondaryHash, int i)
		{
			int resultingHash = (primaryHash + i * secondaryHash) % _hashBits.Count;
			return Math.Abs(resultingHash);
		}
	}
}