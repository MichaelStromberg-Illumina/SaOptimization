using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NirvanaCommon;

namespace Preloader
{
    public static class Preloader
    {
        public static VcfPreloadData GetPositions(List<string> lines, bool checkErrors = true)
        {
            var positions               = new HashSet<int>();
            var positionAlleles         = new List<ulong>();
            var positionAlleleHashTable = new LongHashTable();

            foreach (var line in lines)
            {
                string[] cols      = line.Split('\t');
                int      position  = int.Parse(cols[0]);
                string   refAllele = cols[1];
                string   altAllele = cols[2];

                VariantType variantType    = VariantTypeUtilities.GetVariantType(refAllele, altAllele);
                string      allele         = VariantTypeUtilities.GetAllele(refAllele, altAllele, variantType);
                ulong       positionAllele = PositionAllele.Convert(position, allele, variantType);

                positions.Add(position);
                positionAlleleHashTable.Add(positionAllele);
                positionAlleles.Add(positionAllele);
            }
            
            // Console.WriteLine($"- preloader: {positions.Count:N0} positions, {positionAlleleHashTable.Count:N0} alleles");

            if (checkErrors)
            {
                bool foundError = false;

                if (positionAlleleHashTable.Count != Datasets.NumPedigreeAllelicVariants)
                {
                    Console.WriteLine(
                        $"ERROR: Unexpected number of position-alleles. Expected: {Datasets.NumPedigreeAllelicVariants:N0} vs Observed: {positionAlleleHashTable.Count:N0}");
                    foundError = true;
                }

                if (positions.Count != Datasets.NumPedigreePositions)
                {
                    Console.WriteLine(
                        $"ERROR: Unexpected number of positions. Expected: {Datasets.NumPedigreePositions:N0} vs Observed: {positions.Count:N0}");
                    foundError = true;
                }

                if (foundError) Environment.Exit(1);
            }

            return new VcfPreloadData(positions.ToList(), positionAlleleHashTable, positionAlleles.ToArray());
        }

        public static List<string> GetLines(string tsvPath)
        {
            var lines = new List<string>();

            using (var fileStream = new FileStream(tsvPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fileStream))
            {
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null) break;
                    lines.Add(line);
                }
            }

            return lines;
        }
    }
}