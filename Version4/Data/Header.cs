using NirvanaCommon;

namespace Version4.Data
{
    public sealed class Header
    {
        public readonly string            Identifier;
        public readonly byte              FileFormatVersion;
        public readonly int               NumRefSeqs;
        public readonly GenomeAssembly    Assembly;
        public readonly DataSourceVersion DataSourceVersion;
        public readonly string            JsonKey;
        public readonly byte[]            CompressionDictionary;

        public Header(string identifier, byte fileFormatVersion, int numRefSeqs, GenomeAssembly assembly,
            DataSourceVersion dataSourceVersion, string jsonKey, byte[] compressionDictionary)
        {
            Identifier            = identifier;
            FileFormatVersion     = fileFormatVersion;
            NumRefSeqs            = numRefSeqs;
            Assembly              = assembly;
            DataSourceVersion     = dataSourceVersion;
            JsonKey               = jsonKey;
            CompressionDictionary = compressionDictionary;
        }

        public void Write(ExtendedBinaryWriter writer)
        {
            writer.Write(Identifier);
            writer.Write(FileFormatVersion);
            writer.WriteOpt(NumRefSeqs);
            writer.Write((byte) Assembly);
            writer.Write(JsonKey);

            DataSourceVersion.Write(writer);

            writer.WriteOpt(CompressionDictionary.Length);
            writer.Write(CompressionDictionary);
        }

        public static Header Read(ExtendedBinaryReader reader)
        {
            string identifier        = reader.ReadString();
            byte   fileFormatVersion = reader.ReadByte();
            int    numRefSeqs        = reader.ReadOptInt32();
            var    assembly          = (GenomeAssembly) reader.ReadByte();
            string jsonKey           = reader.ReadString();

            DataSourceVersion dataSourceVersion = DataSourceVersion.Read(reader);

            int numDictBytes = reader.ReadOptInt32();
            byte[] compressionDictionary = reader.ReadBytes(numDictBytes);

            return new Header(identifier, fileFormatVersion, numRefSeqs, assembly, dataSourceVersion, jsonKey,
                compressionDictionary);
        }
    }
}