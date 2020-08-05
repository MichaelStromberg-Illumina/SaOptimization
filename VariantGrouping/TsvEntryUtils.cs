using System.IO;

namespace VariantGrouping
{
    public static class TsvEntryUtils
    {
        public static TsvEntry GetTsvEntry(string line)
        {
            string[] cols = line.Split('\t');
            if (cols.Length != 6) throw new InvalidDataException($"Found an invalid number of columns: {cols.Length}");

            int    position  = int.Parse(cols[0]);
            string refAllele = cols[1];
            string altAllele = cols[2];
            string json      = cols[5];

            return new TsvEntry(position, refAllele, altAllele, json);
        }
    }
}