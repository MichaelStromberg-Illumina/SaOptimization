using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NirvanaCommon;

namespace Preloader
{
    public static class Preloader
    {
        public static (List<int> Positions, LongHashTable PositionAlleles) GetPositions(string tsvPath)
        {
            var positions       = new HashSet<int>();
            var positionAlleles = new LongHashTable();

            using (var fileStream = new FileStream(tsvPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fileStream))
            {
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null) break;

                    string[] cols      = line.Split('\t');
                    int      position  = int.Parse(cols[0]);
                    string   refAllele = cols[1];
                    string   altAllele = cols[2];

                    VariantType variantType    = VariantTypeUtilities.GetVariantType(refAllele, altAllele);
                    string      allele         = VariantTypeUtilities.GetAllele(refAllele, altAllele, variantType);
                    long        positionAllele = PositionAllele.Convert(position, allele, variantType);
                    
                    // Console.WriteLine($"- position: {position:N0}, variant type: {variantType}, REF: {refAllele}, ALT: {altAllele}, allele: {allele}, position-allele: 0x{positionAllele:x16}");
                    // if (positions.Count == 10) break;

                    positions.Add(position);
                    positionAlleles.Add(positionAllele);
                }
            }
            
            Console.WriteLine($"- preloader: {positions.Count:N0} positions, {positionAlleles.Count:N0} alleles");

            bool foundError = false;

            if (positionAlleles.Count != Datasets.NumPedigreeAllelicVariants)
            {
                Console.WriteLine(
                    $"ERROR: Unexpected number of position-alleles. Expected: {Datasets.NumPedigreeAllelicVariants:N0} vs Observed: {positionAlleles.Count:N0}");
                foundError = true;
            }

            if (positions.Count != Datasets.NumPedigreePositions)
            {
                Console.WriteLine(
                    $"ERROR: Unexpected number of positions. Expected: {Datasets.NumPedigreePositions:N0} vs Observed: {positions.Count:N0}");
                foundError = true;
            }

            if (foundError) Environment.Exit(1);
            
            return (positions.ToList(), positionAlleles);
        }
    }
}