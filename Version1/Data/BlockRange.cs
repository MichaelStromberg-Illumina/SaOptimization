namespace Version1.Data
{
    public readonly struct BlockRange
    {
        public readonly long FileOffset;
        public readonly int  NumBlocks;

        public BlockRange(long fileOffset, int numBlocks)
        {
            FileOffset = fileOffset;
            NumBlocks  = numBlocks;
        }
    }
}