using NirvanaCommon;

namespace Version8.Data
{
    public sealed class IndexHeader
    {
        public readonly string Identifier;
        public readonly byte   FileFormatVersion;
        public readonly int    NumRefSeqs;

        public IndexHeader(string identifier, byte fileFormatVersion, int numRefSeqs)
        {
            Identifier        = identifier;
            FileFormatVersion = fileFormatVersion;
            NumRefSeqs        = numRefSeqs;
        }

        public void Write(ExtendedBinaryWriter writer)
        {
            writer.Write(Identifier);
            writer.Write(FileFormatVersion);
            writer.WriteOpt(NumRefSeqs);
        }

        public static IndexHeader Read(ExtendedBinaryReader reader)
        {
            string identifier        = reader.ReadString();
            byte   fileFormatVersion = reader.ReadByte();
            int    numRefSeqs        = reader.ReadOptInt32();

            return new IndexHeader(identifier, fileFormatVersion, numRefSeqs);
        }
    }
}