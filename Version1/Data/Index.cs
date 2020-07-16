namespace Version1.Data
{
    public sealed class Index
    {
        public readonly IndexHeader Header;
        private readonly int _numRefSeqs;

        public Index(IndexHeader header, int numRefSeqs)
        {
            Header      = header;
            _numRefSeqs = numRefSeqs;
        }
    }
}