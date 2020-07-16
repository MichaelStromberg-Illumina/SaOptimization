namespace Version1.Utilities
{
    public static class BlockUtilities
    {
        // public static Block Create(List<TsvEntry> entries, ICompressionAlgorithm zstd)
        // {
        //         
        //     return blocks;
        // }
        //
        // private static int GetLength(in ReadOnlySpan<TsvEntry> entrySpan, in int start, in int maxEntries)
        // {
        //     int end = start + maxEntries - 1;
        //     
        //     if (end >= entrySpan.Length)
        //     {
        //         end = entrySpan.Length - 1;
        //         return end - start + 1;
        //     }
        //
        //     int position     = entrySpan[end].Position;
        //     int nextPosition = entrySpan[end + 1].Position;
        //
        //     if (position != nextPosition) return maxEntries;
        //
        //     // go back to the last position
        //     end--;
        //     while (entrySpan[end].Position == position) end--;
        //     return end - start + 1;
        // }
    }
}