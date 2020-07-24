namespace Version2.Data
{
    public static class SaConstants
    {
        public const int MaxCommonEntries = 65536;
        public const int MaxRareEntries   = 128;

        public const string IndexIdentifier           = "NirvanaIndex";
        public const string AlleleFrequencyIdentifier = "NirvanaAF";
        
        public const string SaPath    = @"E:\Data\Nirvana\NewSA\gnomad_chr1_v2.nsa";
        public const string IndexPath = SaPath + ".idx";
    }
}