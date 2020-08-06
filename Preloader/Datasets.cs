namespace Preloader
{
    public static class Datasets
    {
        public const string PedigreePositionsPath = @"E:\Data\Nirvana\gnomAD_chr1_pedigree_positions.txt";
        public const string PedigreeTsvPath       = @"E:\Data\Nirvana\gnomAD_chr1_pedigree_preload_actual.tsv";

        public const int NumPedigreeAllelicVariants      = 262_528;
        public const int NumPedigreePositionalVariants   = 304_636;
        public const int NumPedigreePositions            = 258_956;
        public const int NumPedigreeIntersectionVariants = 180_445;

        public const string TumorNormalPositionsPath = @"E:\Data\Nirvana\gnomAD_chr1_TN_positions.txt";
        public const string TumorNormalTsvPath       = @"E:\Data\Nirvana\gnomAD_chr1_TN_preload_actual.tsv";
    }
}