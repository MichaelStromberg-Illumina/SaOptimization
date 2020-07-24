using System.Collections.Generic;
using System.IO;
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

            List<PreloadResult> results;
            
            using (FileStream dataStream   = FileUtilities.GetReadStream(dataPath))
            using (FileStream indexStream  = FileUtilities.GetReadStream(indexPath))
            {
                var nsaReader = new NsaReader(dataStream, indexStream);
                results = nsaReader.PreLoad(chromosome, positions);
            }

            return results.Count;
        }
    }
}