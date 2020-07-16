namespace Version1.Nirvana
{
    public sealed class Chromosome
    {
        public readonly string UcscName;
        public readonly string EnsemblName;
        public readonly string RefSeqAccession;
        public readonly string GenBankAccession;
        public readonly int FlankingLength;
        public readonly int Length;
        public readonly ushort Index;

        public const int ShortFlankingLength = 100;

        public Chromosome(string ucscName, string ensemblName, string refSeqAccession, string genBankAccession,
                          int    length,   ushort index)
        {
            UcscName         = ucscName;
            EnsemblName      = ensemblName;
            RefSeqAccession  = refSeqAccession;
            GenBankAccession = genBankAccession;
            Length           = length;
            Index            = index;

            // for short references (< 30 kbp), let's use a shorter flanking length
            const int longFlankingLength      = 5_000;
            const int shortReferenceThreshold = 30_000_000;

            FlankingLength = length < shortReferenceThreshold ? ShortFlankingLength : longFlankingLength;
        }
    }
}