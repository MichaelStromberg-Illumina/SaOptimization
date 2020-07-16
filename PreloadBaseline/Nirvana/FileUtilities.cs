using System.IO;

namespace PreloadBaseline.Nirvana
{
    public static class FileUtilities
    {
        public static FileStream GetReadStream(string path) =>
            new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
    }
}