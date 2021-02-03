using Newtonsoft.Json;
using NirvanaCommon;

namespace Version7.Data
{
    public sealed class GnomadEntry
    {
        // always required
        [JsonProperty(Required = Required.Always)]
        public int coverage;
        [JsonProperty(Required = Required.Always)]
        public int allAc;
        [JsonProperty(Required = Required.Always)]
        public int allAn;
        [JsonProperty(Required = Required.Always)]
        public int allHc;

        // skip
        public bool   lowComplexityRegion;
        public double allAf;
        public double afrAf;
        public double amrAf;
        public double easAf;
        public double finAf;
        public double nfeAf;
        public double othAf;
        public double asjAf;
        public double sasAf;
        public double maleAf;
        public double femaleAf;
        public double controlsAllAf;

        // optional
        public bool failedFilter;

        public int? afrAc;
        public int? afrAn;
        public int? afrHc;

        public int? amrAc;
        public int? amrAn;
        public int? amrHc;

        public int? easAc;
        public int? easAn;
        public int? easHc;

        public int? finAc;
        public int? finAn;
        public int? finHc;

        public int? nfeAc;
        public int? nfeAn;
        public int? nfeHc;

        public int? othAc;
        public int? othAn;
        public int? othHc;

        public int? asjAc;
        public int? asjAn;
        public int? asjHc;

        public int? sasAc;
        public int? sasAn;
        public int? sasHc;

        public int? maleAc;
        public int? maleAn;
        public int? maleHc;

        public int? femaleAc;
        public int? femaleAn;
        public int? femaleHc;

        public int? controlsAllAc;
        public int? controlsAllAn;
        
        public void Write(ExtendedBinaryWriter writer)
        {
            bool hasAfr = afrAn != null;
            bool hasAmr = amrAn != null;
            bool hasAsj = asjAn != null;
            bool hasEas = easAn != null;
            bool hasFin = finAn != null;
            bool hasNfe = nfeAn != null;
            bool hasOth = othAn != null;
            bool hasSas = sasAn != null;

            bool hasMale     = maleAn        != null;
            bool hasFemale   = femaleAn      != null;
            bool hasControls = controlsAllAn != null;

            ushort flags = GetFlags(hasAfr, hasAmr, hasAsj, hasEas, hasFin, hasNfe, hasOth, hasSas, hasMale, hasFemale,
                hasControls,                failedFilter);

            writer.Write(flags);
            
            writer.WriteOpt(coverage);
            WritePopulation(writer, allAc, allAn, allHc);

            if (hasAfr) WritePopulation(writer,    afrAc.Value,    afrAn.Value,    afrHc.Value);
            if (hasAmr) WritePopulation(writer,    amrAc.Value,    amrAn.Value,    amrHc.Value);
            if (hasAsj) WritePopulation(writer,    asjAc.Value,    asjAn.Value,    asjHc.Value);
            if (hasEas) WritePopulation(writer,    easAc.Value,    easAn.Value,    easHc.Value);
            if (hasFin) WritePopulation(writer,    finAc.Value,    finAn.Value,    finHc.Value);
            if (hasNfe) WritePopulation(writer,    nfeAc.Value,    nfeAn.Value,    nfeHc.Value);
            if (hasOth) WritePopulation(writer,    othAc.Value,    othAn.Value,    othHc.Value);
            if (hasSas) WritePopulation(writer,    sasAc.Value,    sasAn.Value,    sasHc.Value);
            if (hasMale) WritePopulation(writer,   maleAc.Value,   maleAn.Value,   maleHc.Value);
            if (hasFemale) WritePopulation(writer, femaleAc.Value, femaleAn.Value, femaleHc.Value);
            
            if (hasControls)
            {
                writer.WriteOpt(controlsAllAc.Value);
                writer.WriteOpt(controlsAllAn.Value);
            }
        }

        private static void WritePopulation(ExtendedBinaryWriter writer, int ac, int an, int hc)
        {
            writer.WriteOpt(ac);
            writer.WriteOpt(an);
            writer.WriteOpt(hc);
        }

        private const int AfrMask = 1;
        private const int AmrMask = 2;
        private const int AsjMask = 4;
        private const int EasMask = 8;
        private const int FinMask = 16;
        private const int NfeMask = 32;
        private const int OthMask = 64;
        
        private const int SasMask      = 128;
        private const int MaleMask     = 256;
        private const int FemaleMask   = 512;
        private const int ControlsMask = 1024;
        private const int FilterMask   = 2048;
        
        private static ushort GetFlags(bool hasAfr, bool hasAmr, bool hasAsj, bool hasEas, bool hasFin, bool hasNfe, bool hasOth,
            bool hasSas, bool hasMale, bool hasFemale, bool hasControls, bool failedFilter)
        {
            ushort flags = 0;

            if (hasAfr) flags |= AfrMask;
            if (hasAmr) flags |= AmrMask;
            if (hasAsj) flags |= AsjMask;
            if (hasEas) flags |= EasMask;
            if (hasFin) flags |= FinMask;
            if (hasNfe) flags |= NfeMask;
            if (hasOth) flags |= OthMask;
            if (hasSas) flags |= SasMask;

            if (hasMale) flags      |= MaleMask;
            if (hasFemale) flags    |= FemaleMask;
            if (hasControls) flags  |= ControlsMask;
            if (failedFilter) flags |= FilterMask;
            
            return flags;
        }
    }
}