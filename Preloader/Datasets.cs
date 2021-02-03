using System.IO;
using System.Runtime.InteropServices;

namespace Preloader
{
    public static class Datasets
    {
        private static readonly string _nirvanaDir;
        public static string PedigreeTsvPath => Path.Combine(_nirvanaDir, "gnomAD_chr1_pedigree_preload_actual.tsv");

        // describe the pedigree dataset (VCF)
        public const int NumPedigreeVariants  = 262_528;
        public const int NumPedigreePositions = 258_956;

        // describe the intersection between the pedigree dataset (VCF) and gnomAD
        public const int NumPedigreePreloadedVariants           = 180_445;
        public const int NumPedigreePreloadedPositions          = 187_064;
        public const int NumPedigreePreloadedPositionalVariants = 304_636;

        public const string TumorNormalPositionsPath = @"E:\Data\Nirvana\gnomAD_chr1_TN_positions.txt";
        public const string TumorNormalTsvPath       = @"E:\Data\Nirvana\gnomAD_chr1_TN_preload_actual.tsv";

        static Datasets()
        {
            _nirvanaDir = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? @"E:\Data\Nirvana"
                : "/e/Data/Nirvana";
        }
    }
}