namespace Version1.Data
{
    public sealed class Header
    {
        public readonly string Identifier;
        public readonly byte FileFormatVersion;

        public Header(string identifier, byte fileFormatVersion)
        {
            Identifier        = identifier;
            FileFormatVersion = fileFormatVersion;
        }
    }
}