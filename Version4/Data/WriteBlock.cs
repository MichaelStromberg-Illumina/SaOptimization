using NirvanaCommon;

namespace Version4.Data
{
    public class WriteBlock : Block
    {
        public readonly int LastPosition;
        public readonly int Index;

        public WriteBlock(byte[] compressedBytes, int numCompressedBytes, int numUncompressedBytes, int lastPosition, int index) : base(
            compressedBytes, numCompressedBytes, numUncompressedBytes)
        {
            LastPosition = lastPosition;
            Index        = index;
        }

        public void Write(ExtendedBinaryWriter writer)
        {
            writer.WriteOpt(NumCompressedBytes);
            writer.WriteOpt(NumUncompressedBytes - NumCompressedBytes);
            writer.Write(CompressedBytes, 0, NumCompressedBytes);
        }
    }
}