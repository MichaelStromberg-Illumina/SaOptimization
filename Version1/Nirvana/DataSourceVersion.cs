using System;

namespace Version1.Nirvana
{
    public sealed class DataSourceVersion
    {
        public readonly string Name;
        public readonly string Description;
        public readonly string Version;
        public readonly DateTime ReleaseDate;

        public DataSourceVersion(string name, string version, DateTime releaseDate, string description)
        {
            Name        = name;
            Description = description;
            ReleaseDate = releaseDate;
            Version     = version;
        }
    }
}