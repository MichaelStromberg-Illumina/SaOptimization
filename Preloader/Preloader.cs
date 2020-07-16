using System.Collections.Generic;
using System.IO;

namespace Preloader
{
    public static class Preloader
    {
        public static List<PositionAllele> GetPositionAlleles()
        {
            var positionAlleles = new List<PositionAllele>();

            using (var fileStream = new FileStream(@"E:\Data\Nirvana\gnomAD_chr1_preload.tsv", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fileStream))
            {
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null) break;

                    string[] cols = line.Split('\t');

                    int    position  = int.Parse(cols[0]);
                    string refAllele = cols[1];
                    string altAllele = cols[2];

                    positionAlleles.Add(new PositionAllele(position, refAllele, altAllele));
                }
            }

            return positionAlleles;
        }

        public static List<int> GetPositions()
        {
            List<PositionAllele> positionAlleles = GetPositionAlleles();
            var                  positions       = new List<int>(positionAlleles.Count);
            foreach (PositionAllele positionAllele in positionAlleles) positions.Add(positionAllele.Position);
            return positions;
        }
    }
}