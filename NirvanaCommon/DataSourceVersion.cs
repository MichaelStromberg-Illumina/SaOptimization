using System;

namespace NirvanaCommon
{
    public sealed class DataSourceVersion
    {
        public readonly string   Name;
        public readonly string   Description;
        public readonly string   Version;
        public readonly DateTime ReleaseDate;

        public DataSourceVersion(string name, string version, DateTime releaseDate, string description)
        {
            Name        = name;
            Description = description;
            ReleaseDate = releaseDate;
            Version     = version;
        }

        public void Write(ExtendedBinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(Description);
            writer.Write(Version);
            writer.WriteOpt(ReleaseDate.Ticks);
        }

        public static DataSourceVersion Read(ExtendedBinaryReader reader)
        {
            string name         = reader.ReadString();
            string description  = reader.ReadString();
            string version      = reader.ReadString();
            long   releaseTicks = reader.ReadOptInt64();

            return new DataSourceVersion(name, version, new DateTime(releaseTicks), description);
        }
    }
}