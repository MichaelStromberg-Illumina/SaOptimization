using System.IO;

namespace Version7.Utilities
{
    public static class SaPath
    {
        public static (string SaPath, string IndexPath) GetPaths(string saDir, string threshold, int commonBlockSize,
            int rareBlockSize)
        {
            string saPath    = Path.Combine(saDir, $"gnomad_chr1_v7_{threshold}_{commonBlockSize}_{rareBlockSize}.nsa");
            string indexPath = saPath + ".idx";
            return (saPath, indexPath);
        }
    }
}