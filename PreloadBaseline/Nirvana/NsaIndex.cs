using System.Collections.Generic;
using System.IO;

namespace PreloadBaseline.Nirvana
{
    public sealed class NsaIndex
    {
        private readonly Dictionary<ushort, List<NsaIndexBlock>> _chromBlocks;
        private ushort _chromIndex = ushort.MaxValue;

        public readonly GenomeAssembly Assembly;
        public readonly DataSourceVersion Version;
        public readonly string JsonKey;
        public readonly int SchemaVersion;
        public readonly bool IsArray;
        public readonly bool MatchByAllele;
        public readonly bool IsPositional;


        public NsaIndex(Stream stream)
        {
            using (var memStream = new MemoryStream())
            using (var memReader = new ExtendedBinaryReader(memStream))
            {
                stream.CopyTo(memStream); //reading all bytes in stream to memStream
                memStream.Position = 0;

                Assembly      = (GenomeAssembly) memReader.ReadByte();
                Version       = DataSourceVersion.Read(memReader);
                JsonKey       = memReader.ReadAsciiString();
                MatchByAllele = memReader.ReadBoolean();
                IsArray       = memReader.ReadBoolean();
                SchemaVersion = memReader.ReadOptInt32();
                IsPositional  = memReader.ReadBoolean();

                var chromCount = memReader.ReadOptInt32();
                _chromBlocks = new Dictionary<ushort, List<NsaIndexBlock>>(chromCount);
                for (var i = 0; i < chromCount; i++)
                {
                    var chromIndex = memReader.ReadOptUInt16();
                    var chunkCount = memReader.ReadOptInt32();
                    _chromBlocks[chromIndex] = new List<NsaIndexBlock>(chunkCount);
                    for (var j = 0; j < chunkCount; j++)
                        _chromBlocks[chromIndex].Add(new NsaIndexBlock(memReader));
                }
            }
        }

        public long GetFileLocation(ushort chromIndex, int start)
        {
            if (_chromBlocks == null || !_chromBlocks.TryGetValue(chromIndex, out var chunks)) return -1;
            var index = BinarySearch(chunks, start);

            if (index < 0) return -1;
            return chunks[index].FilePosition;
        }

        private static int BinarySearch(List<NsaIndexBlock> chunks, int position)
        {
            var begin = 0;
            int end   = chunks.Count - 1;

            while (begin <= end)
            {
                int index = begin + (end - begin >> 1);

                int ret = chunks[index].CompareTo(position);
                if (ret == 0) return index;
                if (ret < 0) begin = index + 1;
                else end           = index - 1;
            }

            return ~begin;
        }
    }
}