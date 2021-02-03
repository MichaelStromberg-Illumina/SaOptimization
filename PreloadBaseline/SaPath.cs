using System.IO;

namespace PreloadBaseline
{
    public static class SaPath
    {
        public static (string SaPath, string IndexPath) GetPaths(string saDir)
        {
            string saPath    = Path.Combine(saDir, "gnomAD_2.1.nsa");
            string indexPath = saPath + ".idx";
            return (saPath, indexPath);
        }
    }
}