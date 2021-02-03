namespace Version7.Data
{
    public readonly struct IndexEntry
    {
        public readonly int  End;
        public readonly long Offset;

        public IndexEntry(int end, long offset)
        {
            End    = end;
            Offset = offset;
        }
    }
}