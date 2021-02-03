using System.IO;

namespace Version3.Utilities
{
    public static class SaPath
    {
        public static (string SaPath, string IndexPath) GetPaths(string saDir)
        {
            string saPath    = Path.Combine(saDir, "gnomad_chr1_v3.nsa");
            string indexPath = saPath + ".idx";
            return (saPath, indexPath);
        }
    }
}