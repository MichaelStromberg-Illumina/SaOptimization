using System.IO;

namespace Version8.Utilities
{
    public static class SaPath
    {
        public static (string SaPath, string IndexPath) GetPaths(string saDir, string threshold, int commonBlockSize,
            int rareBlockSize)
        {
            string saPath    = Path.Combine(saDir, $"dbsnp_chr1_v8_{threshold}_{commonBlockSize}_{rareBlockSize}.nsa");
            string indexPath = saPath + ".idx";
            return (saPath, indexPath);
        }
    }
}