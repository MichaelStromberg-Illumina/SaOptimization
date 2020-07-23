using System.IO;

namespace VariantGrouping
{
    public static class TsvEntryUtils
    {
        private const double CommonThreshold = 0.05;
        
        public static TsvEntry GetTsvEntry(string line)
        {
            string[] cols = line.Split('\t');
            if (cols.Length != 6) throw new InvalidDataException($"Found an invalid number of columns: {cols.Length}");

            int    position  = int.Parse(cols[0]);
            string refAllele = cols[1];
            string altAllele = cols[2];
            double popMax    = double.Parse(cols[4]);
            string json      = cols[5];

            bool isCommon = popMax >= CommonThreshold;

            return new TsvEntry(position, refAllele, altAllele, json, isCommon);
        }
    }
}