using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using NirvanaCommon;

namespace CreateGnomadVersion7
{
    public static class AlleleIndex
    {
        public static async Task<(ulong[] PositionAlleles, ulong[] CommonPositionAlleles)> GetAllelesAsync(
            string commonTsvPath, string rareTsvPath)
        {
            var alleles               = new HashSet<string>();
            var commonPositionAlleles = new List<ulong>();
            var rarePositionAlleles   = new List<ulong>();

            await alleles.AddFromTsvAsync(commonTsvPath, commonPositionAlleles).ConfigureAwait(false);
            await alleles.AddFromTsvAsync(rareTsvPath,   rarePositionAlleles).ConfigureAwait(false);

            var positionAlleles = new HashSet<ulong>();
            foreach (ulong pa in commonPositionAlleles) positionAlleles.Add(pa);
            foreach (ulong pa in rarePositionAlleles) positionAlleles.Add(pa);

            ulong[] sortedPositionAlleles       = positionAlleles.OrderBy(x => x).ToArray();
            ulong[] sortedCommonPositionAlleles = commonPositionAlleles.OrderBy(x => x).ToArray();

            return (sortedPositionAlleles, sortedCommonPositionAlleles);
        }

        private static async Task AddFromTsvAsync(this HashSet<string> alleles, string tsvPath, List<ulong> positionAlleles)
        {
            using (var reader = new StreamReader(new GZipStream(FileUtilities.GetReadStream(tsvPath),
                CompressionMode.Decompress)))
            {
                while (true)
                {
                    string line = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line)) break;

                    string[] cols = line.Split('\t', 4);
                    if (cols.Length != 4)
                        throw new InvalidDataException($"Found an invalid number of columns: {cols.Length}");

                    int    position  = int.Parse(cols[0]);
                    string refAllele = cols[1];
                    string altAllele = cols[2];
                    
                    VariantType variantType = VariantTypeUtilities.GetVariantType(refAllele, altAllele);
                    VariantTypeUtilities.CheckVariantType(variantType);

                    string allele = variantType == VariantType.deletion ? refAllele : altAllele;
                    
                    ulong positionAllele = PositionAllele.Convert(position, allele, variantType);
                    positionAlleles.Add(positionAllele);

                    alleles.Add(allele);
                }
            }
        }
    }
}