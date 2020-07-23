using System.Collections.Generic;
using System.IO;

namespace Preloader
{
    public static class Preloader
    {
        public static List<PositionAllele> GetPositionAlleles(string tsvPath)
        {
            var positionAlleles = new List<PositionAllele>();

            using (var fileStream = new FileStream(tsvPath, FileMode.Open, FileAccess.Read, FileShare.Read))
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

        public static List<int> GetPositions(string tsvPath)
        {
            List<PositionAllele> positionAlleles = GetPositionAlleles(tsvPath);
            var                  positions       = new List<int>(positionAlleles.Count);
            foreach (PositionAllele positionAllele in positionAlleles) positions.Add(positionAllele.Position);
            return positions;
        }
    }
}