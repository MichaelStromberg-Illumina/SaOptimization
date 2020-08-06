using NirvanaCommon;

namespace PreloadBaseline.Nirvana
{
    public sealed class DataSourceVersion
    {
        public string Name             { get; }
        public string Description      { get; }
        public string Version          { get; }
        public long   ReleaseDateTicks { get; }

        public DataSourceVersion(string name, string version, long releaseDateTicks, string description = null)
        {
            Name             = name;
            Description      = description;
            Version          = version;
            ReleaseDateTicks = releaseDateTicks;
        }

        public static DataSourceVersion Read(ExtendedBinaryReader reader)
        {
            var name             = reader.ReadAsciiString();
            var version          = reader.ReadAsciiString();
            var releaseDateTicks = reader.ReadOptInt64();
            var description      = reader.ReadAsciiString();
            return new DataSourceVersion(name, version, releaseDateTicks, description);
        }
        
        public override string ToString() => "dataSource=" + Name + ",version:" + Version + ",release date:";
    }
}