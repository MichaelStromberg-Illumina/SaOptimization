using System;
using NirvanaCommon;

namespace VariantGrouping
{
    public static class GnomAD
    {
        public static readonly DateTime ReleaseDate = DateTime.Parse("2018-10-17");

        public const string JsonKey        = "gnomad";
        public const string DictionaryPath = @"E:\Data\Nirvana\NewSA\gnomad.dict";

        public static readonly DataSourceVersion DataSourceVersion = new DataSourceVersion("gnomAD", "2.1", ReleaseDate,
            "Allele frequencies from Genome Aggregation Database (gnomAD)");
    }
}