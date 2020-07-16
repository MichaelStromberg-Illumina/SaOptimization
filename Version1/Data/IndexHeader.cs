using Version1.Nirvana;

namespace Version1.Data
{
    public sealed class IndexHeader
    {
        public readonly string Identifier;
        public readonly byte FileFormatVersion;
        public readonly GenomeAssembly GenomeAssembly;
        public readonly DataSourceVersion DataSourceVersion;
        public readonly string JsonKey;
        public readonly byte[] CompressionDictionary;

        public IndexHeader(string            identifier,
                           byte              fileFormatVersion,
                           GenomeAssembly    genomeAssembly,
                           DataSourceVersion dataSourceVersion,
                           string            jsonKey,
                           byte[]            compressionDictionary)
        {
            Identifier            = identifier;
            FileFormatVersion     = fileFormatVersion;
            GenomeAssembly        = genomeAssembly;
            DataSourceVersion     = dataSourceVersion;
            JsonKey               = jsonKey;
            CompressionDictionary = compressionDictionary;
        }
    }
}