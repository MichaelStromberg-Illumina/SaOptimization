using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Compression.Algorithms;
using Compression.Data;
using NirvanaCommon;
using Version5.Data;

namespace CreateGnomadVersion5
{
    public static class AlleleIndex
    {
        public static async
            Task<(WriteBlock AlleleBlock, Dictionary<string, int> AlleleToIndex, ulong[] PositionAlleles, ulong[]
                CommonPositionAlleles)> GetAllelesAsync(string commonTsvPath, string rareTsvPath)
        {
            var alleles               = new HashSet<string>();
            var commonPositionAlleles = new List<ulong>();
            var rarePositionAlleles   = new List<ulong>();

            await alleles.AddFromTsvAsync(commonTsvPath, commonPositionAlleles).ConfigureAwait(false);
            await alleles.AddFromTsvAsync(rareTsvPath, rarePositionAlleles).ConfigureAwait(false);

            string[] sortedAlleles = alleles.OrderBy(s => s.Length).ThenBy(s => s).ToArray();

            var positionAlleles = new HashSet<ulong>();
            foreach (ulong pa in commonPositionAlleles) positionAlleles.Add(pa);
            foreach (ulong pa in rarePositionAlleles) positionAlleles.Add(pa);

            ulong[] sortedPositionAlleles       = positionAlleles.ToArray();
            ulong[] sortedCommonPositionAlleles = commonPositionAlleles.ToArray();

            int numAlleles = sortedAlleles.Length;
            var alleleToIndex = new Dictionary<string, int>(numAlleles);
            for (var i = 0; i < numAlleles; i++) alleleToIndex[sortedAlleles[i]] = i;

            WriteBlock alleleBlock = CreateBlock(sortedAlleles);
            
            return (alleleBlock, alleleToIndex, sortedPositionAlleles, sortedCommonPositionAlleles);
        }

        private static WriteBlock CreateBlock(string[] alleles)
        {
            byte[] bytes;
            int numBytes;

            using (var stream = new MemoryStream())
            using (var writer = new ExtendedBinaryWriter(stream))
            {
                writer.WriteOpt(alleles.Length);
                foreach (string allele in alleles) writer.Write(allele);
                bytes = stream.ToArray();
                numBytes = (int)stream.Position;
            }

            var context = new ZstdContext(CompressionMode.Compress);
            
            int compressedBufferSize = ZstandardStatic.GetCompressedBufferBounds(numBytes);
            var compressedBytes      = new byte[compressedBufferSize];
            
            int numCompressedBytes = ZstandardStatic.Compress(bytes, numBytes, compressedBytes,
                compressedBufferSize, context);
            double percent = numCompressedBytes / (double) numBytes * 100.0;
            
            Console.WriteLine($"  - uncompressed: {numBytes:N0} bytes, compressed: {numCompressedBytes:N0} bytes ({percent:0.0}%)");

            return new WriteBlock(compressedBytes, numCompressedBytes, numBytes, 0, 0);
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