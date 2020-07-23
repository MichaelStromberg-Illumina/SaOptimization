using System.IO;

namespace NirvanaCommon
{
    public static class FileUtilities
    {
        public static FileStream GetReadStream(string path) =>
            new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

        public static FileStream GetWriteStream(string path) =>
            new FileStream(path, FileMode.Create, FileAccess.Write);
    }
}