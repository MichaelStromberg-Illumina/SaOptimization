using System.Collections.Generic;
using System.IO;
using NirvanaCommon;
using PreloadBaseline.Nirvana;

namespace PreloadBaseline
{
    public static class Baseline
    {
        public static int Preload(Chromosome chromosome, string saPath, string indexPath, List<int> positions)
        {
            List<PreloadResult> results;
            
            using (FileStream dataStream   = FileUtilities.GetReadStream(saPath))
            using (FileStream indexStream  = FileUtilities.GetReadStream(indexPath))
            {
                var nsaReader = new NsaReader(dataStream, indexStream);
                results = nsaReader.PreLoad(chromosome, positions);
            }

            return results.Count;
        }
    }
}