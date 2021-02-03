using System.IO;

namespace Version1.Utilities
{
    public static class SaPath
    {
        public static (string SaPath, string IndexPath) GetPaths(string saDir, string threshold)
        {
            string saPath    = Path.Combine(saDir, $"gnomad_chr1_v1_{threshold}.nsa");
            string indexPath = saPath + ".idx";
            return (saPath, indexPath);
        }
    }
}