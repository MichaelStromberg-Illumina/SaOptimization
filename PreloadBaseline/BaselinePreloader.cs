using System.Collections.Generic;
using System.IO;
using System.Linq;
using NirvanaCommon;
using PreloadBaseline.Nirvana;

namespace PreloadBaseline
{
    public static class Baseline
    {
        public static int Preload(Chromosome chromosome, List<int> positions)
        {
            const string dataPath  = @"E:\Data\Nirvana\Data\SupplementaryAnnotation\GRCh37_gnomAD\gnomAD_2.1.nsa";
            const string indexPath = dataPath + ".idx";
            
            const string outputPath = @"E:\Data\Nirvana\gnomAD_chr1_pedigree_preload_baseline.tsv";

            List<PreloadResult> results;
            
            using (FileStream dataStream   = FileUtilities.GetReadStream(dataPath))
            using (FileStream indexStream  = FileUtilities.GetReadStream(indexPath))
            using (FileStream outputStream = FileUtilities.GetWriteStream(outputPath))
            using (StreamWriter writer     = new StreamWriter(outputStream))
            {
                writer.NewLine = "\n";
                var nsaReader = new NsaReader(dataStream, indexStream);
                results = nsaReader.PreLoad(chromosome, positions);

                foreach (PreloadResult result in results.OrderBy(x=>x.Position).ThenBy(x=>x.Json)) writer.WriteLine(result);
            }

            return results.Count;
        }
    }
}