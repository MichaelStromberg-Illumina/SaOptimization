namespace CreateGnomadVersion2
{
    public sealed class ConvertedData
    {
        public readonly int    LastPosition;
        public readonly int    NumEntries;
        public readonly byte[] Bytes;
        public readonly int    Index;

        public ConvertedData(int lastPosition, int numEntries, byte[] bytes, int index)
        {
            LastPosition = lastPosition;
            NumEntries   = numEntries;
            Bytes        = bytes;
            Index        = index;
        }
    }
}