using System.IO;

namespace Version4.Utilities
{
    public static class SaPath
    {
        public static (string SaPath, string IndexPath) GetPaths(string saDir)
        {
            string saPath    = Path.Combine(saDir, "gnomad_chr1_v4.nsa");
            string indexPath = saPath + ".idx";
            return (saPath, indexPath);
        }
    }
}