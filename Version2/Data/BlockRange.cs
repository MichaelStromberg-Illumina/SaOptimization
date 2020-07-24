namespace Version2.Data
{
    public readonly struct BlockRange
    {
        public readonly long FileOffset;
        public readonly int  NumBlocks;
        public readonly int  Start;
        public readonly int  End;

        public BlockRange(long fileOffset, int numBlocks, int start, int end)
        {
            FileOffset = fileOffset;
            NumBlocks  = numBlocks;
            Start      = start;
            End        = end;
        }

        public override string ToString() => $"- file offset: {FileOffset:N0}, # of blocks: {NumBlocks}, index: [{Start} - {End}]";
    }
}