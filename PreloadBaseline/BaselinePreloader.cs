using System.Collections.Generic;
using System.IO;
using PreloadBaseline.Nirvana;

namespace PreloadBaseline
{
    public static class Baseline
    {
        public static int Preload(IChromosome chr1, List<int> positions)
        {
            const string dataPath  = @"E:\Data\Nirvana\Data\SupplementaryAnnotation\GRCh37_gnomAD\gnomAD_2.1.nsa";
            const string indexPath = dataPath + ".idx";

            var annotations = new List<(string refAllele, string altAllele, string annotation)>();

            using (FileStream dataStream = FileUtilities.GetReadStream(dataPath))
            using (FileStream indexStream = FileUtilities.GetReadStream(indexPath))
            {
                var nsaReader = new NsaReader(dataStream, indexStream);

                nsaReader.PreLoad(chr1, positions);
                foreach (int position in positions) nsaReader.GetAnnotation(position, annotations);
            }

            return annotations.Count;
        }
    }
}