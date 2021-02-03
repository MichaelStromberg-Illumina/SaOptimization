using System.Runtime.InteropServices;

namespace NirvanaCommon
{
    public static class SupplementaryAnnotation
    {
        public static string Directory =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? @"E:\Data\Nirvana\NewSA"
                : "/e/Data/Nirvana/NewSA";
        
        public static string DevelopDirectory =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? @"E:\Data\Nirvana\Data\SupplementaryAnnotation\GRCh37_gnomAD"
                : "/e/Data/Nirvana/Data/SupplementaryAnnotation/GRCh37_gnomAD";
    }
}