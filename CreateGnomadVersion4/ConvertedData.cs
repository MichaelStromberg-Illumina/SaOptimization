namespace CreateGnomadVersion4
{
    public sealed class ConvertedData
    {
        public readonly int    LastPosition;
        public readonly byte[] Bytes;
        public readonly int    Index;

        public ConvertedData(int lastPosition, byte[] bytes, int index)
        {
            LastPosition = lastPosition;
            Bytes        = bytes;
            Index        = index;
        }
    }
}