using System;

namespace PreloadBaseline.Nirvana
{
    public sealed class Chromosome : IChromosome, IComparable<IChromosome>
    {
        public string UcscName { get; }
        public string EnsemblName { get; }
        public string RefSeqAccession { get; }
        public string GenBankAccession { get; }
        public int FlankingLength { get; }
        public int Length { get; }
        public ushort Index { get; }

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

        public bool Equals(IChromosome other) => Index == other.Index && Length == other.Length;

        public int CompareTo(IChromosome other) =>
            Index == other.Index ? Length.CompareTo(other.Length) : Index.CompareTo(other.Index);
    }
}