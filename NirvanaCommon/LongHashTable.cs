using System;
using System.Runtime.CompilerServices;

namespace NirvanaCommon
{
#nullable enable
    public sealed class LongHashTable
    {
        private int      _count;
        private int[]?   _buckets;
        private Entry[]? _entries;

        private int   _freeList;
        private int   _freeCount;
        private ulong _fastModMultiplier;

        private const int StartOfFreeList = -3;

        private void Initialize(int capacity)
        {
            int size    = PrimeUtilities.GetPrime(capacity);
            var buckets = new int[size];
            var entries = new Entry[size];

            // Assign member variables after both arrays are allocated to guard against corruption from OOM if second fails.
            _freeList = -1;
            _buckets  = buckets;
            _entries  = entries;

            _fastModMultiplier = PrimeUtilities.GetFastModMultiplier((uint) size);
        }

        public unsafe void Add(long value)
        {
            if (_buckets == null) Initialize(0);

            Entry[]? entries = _entries;

            uint    collisionCount = 0;
            ref int bucket         = ref Unsafe.AsRef<int>(null);

            int hashCode =value.GetHashCode();
            bucket = ref GetBucketRef(hashCode);

            int i = bucket - 1; // Value in _buckets is 1-based

            while (i >= 0)
            {
#pragma warning disable 8602
                ref Entry entry = ref entries[i];
#pragma warning restore 8602
                if (entry.HashCode == hashCode && entry.Value == value) return;

                i = entry.Next;

                collisionCount++;
                if (collisionCount > (uint) entries.Length)
                {
                    // The chain of entries forms a loop, which means a concurrent update has happened.
                    throw new InvalidOperationException("Concurrent Operations Not Supported");
                }
            }

            int index;
            if (_freeCount > 0)
            {
                index = _freeList;
                _freeCount--;
#pragma warning disable 8602
                _freeList = StartOfFreeList - entries[_freeList].Next;
#pragma warning restore 8602
            }
            else
            {
                int count = _count;
#pragma warning disable 8602
                if (count == entries.Length)
#pragma warning restore 8602
                {
                    Resize();
                    bucket = ref GetBucketRef(hashCode);
                }

                index   = count;
                _count  = count + 1;
                entries = _entries;
            }

            {
                ref Entry entry = ref entries![index];
                entry.HashCode = hashCode;
                entry.Next     = bucket - 1; // Value in _buckets is 1-based
                entry.Value    = value;
                bucket         = index + 1;
            }
        }

        private void Resize() => Resize(PrimeUtilities.ExpandPrime(_count));

        private void Resize(int newSize)
        {
            var entries = new Entry[newSize];

            int count = _count;
#pragma warning disable 8604
            Array.Copy(_entries, entries, count);
#pragma warning restore 8604

            _buckets           = new int[newSize];
            _fastModMultiplier = PrimeUtilities.GetFastModMultiplier((uint) newSize);

            for (var i = 0; i < count; i++)
            {
                ref Entry entry = ref entries[i];
                if (entry.Next < -1) continue;
                ref int bucket = ref GetBucketRef(entry.HashCode);
                entry.Next = bucket - 1; // Value in _buckets is 1-based
                bucket     = i      + 1;
            }

            _entries = entries;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref int GetBucketRef(int hashCode)
        {
            int[] buckets = _buckets!;
            return ref buckets[PrimeUtilities.FastMod((uint)hashCode, (uint) buckets.Length, _fastModMultiplier)];
        }

        public bool Contains(long value)
        {
            int[]? buckets = _buckets;
            if (buckets == null) return false;
            
            Entry[]? entries       = _entries;
            var      numCollisions = 0;
            int      hashCode      = value.GetHashCode();
            int      i             = GetBucketRef(hashCode) - 1; // Value in _buckets is 1-based

            while (i >= 0)
            {
#pragma warning disable 8602
                ref Entry entry = ref entries[i];
#pragma warning restore 8602
                if (entry.HashCode == hashCode && entry.Value == value) return true;
                i = entry.Next;

                numCollisions++;
                if (numCollisions > entries.Length)
                    throw new InvalidOperationException("Concurrent operations not supported.");
            }

            return false;
        }

        public int Count => _count - _freeCount;

        private struct Entry
        {
            public int HashCode;
            public int  Next;
            public long Value;
        }
    }
}