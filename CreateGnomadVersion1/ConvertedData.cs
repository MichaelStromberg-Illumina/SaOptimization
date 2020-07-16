namespace CreateGnomadVersion1
{
    public sealed class ConvertedData
    {
        public readonly int    LastPosition;
        public readonly int    NumEntries;
        public readonly byte[] Bytes;

        public ConvertedData(int lastPosition, int numEntries, byte[] bytes)
        {
            LastPosition = lastPosition;
            NumEntries   = numEntries;
            Bytes        = bytes;
        }
    }
}