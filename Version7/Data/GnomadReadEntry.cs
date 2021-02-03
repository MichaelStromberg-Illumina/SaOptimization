using System;
using Version7.IO;

namespace Version7.Data
{
    public sealed class GnomadReadEntry
    {
        public int  coverage;
        public bool failedFilter;

        public int allAc;
        public int allAn;
        public int allHc;

        public bool hasAfr;
        public int  afrAc;
        public int  afrAn;
        public int  afrHc;

        public bool hasAmr;
        public int  amrAc;
        public int  amrAn;
        public int  amrHc;

        public bool hasAsj;
        public int  asjAc;
        public int  asjAn;
        public int  asjHc;

        public bool hasEas;
        public int  easAc;
        public int  easAn;
        public int  easHc;

        public bool hasFin;
        public int  finAc;
        public int  finAn;
        public int  finHc;

        public bool hasNfe;
        public int  nfeAc;
        public int  nfeAn;
        public int  nfeHc;

        public bool hasOth;
        public int  othAc;
        public int  othAn;
        public int  othHc;

        public bool hasSas;
        public int  sasAc;
        public int  sasAn;
        public int  sasHc;

        public bool hasMale;
        public int  maleAc;
        public int  maleAn;
        public int  maleHc;

        public bool hasFemale;
        public int  femaleAc;
        public int  femaleAn;
        public int  femaleHc;

        public bool hasControls;
        public int  controlsAllAc;
        public int  controlsAllAn;
        
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

        public void Read(ref ReadOnlySpan<byte> byteSpan)
        {
            ushort flags = SpanBufferBinaryReader.ReadUInt16(ref byteSpan);

            hasAfr = (flags & AfrMask) != 0;
            hasAmr = (flags & AmrMask) != 0;
            hasAsj = (flags & AsjMask) != 0;
            hasEas = (flags & EasMask) != 0;
            hasFin = (flags & FinMask) != 0;
            hasNfe = (flags & NfeMask) != 0;
            hasOth = (flags & OthMask) != 0;
            hasSas = (flags & SasMask) != 0;

            hasMale      = (flags & MaleMask)     != 0;
            hasFemale    = (flags & FemaleMask)   != 0;
            hasControls  = (flags & ControlsMask) != 0;
            failedFilter = (flags & FilterMask)   != 0;

            coverage              = SpanBufferBinaryReader.ReadOptInt32(ref byteSpan);
            (allAc, allAn, allHc) = ReadPopulation(ref byteSpan);

            if (hasAfr) (afrAc, afrAn, afrHc)             = ReadPopulation(ref byteSpan);
            if (hasAmr) (amrAc, amrAn, amrHc)             = ReadPopulation(ref byteSpan);
            if (hasAsj) (asjAc, asjAn, asjHc)             = ReadPopulation(ref byteSpan);
            if (hasEas) (easAc, easAn, easHc)             = ReadPopulation(ref byteSpan);
            if (hasFin) (finAc, finAn, finHc)             = ReadPopulation(ref byteSpan);
            if (hasNfe) (nfeAc, nfeAn, nfeHc)             = ReadPopulation(ref byteSpan);
            if (hasOth) (othAc, othAn, othHc)             = ReadPopulation(ref byteSpan);
            if (hasSas) (sasAc, sasAn, sasHc)             = ReadPopulation(ref byteSpan);
            if (hasMale) (maleAc, afrAn, afrHc)           = ReadPopulation(ref byteSpan);
            if (hasFemale) (femaleAc, femaleAn, femaleHc) = ReadPopulation(ref byteSpan);

            if (hasControls)
            {
                controlsAllAc = SpanBufferBinaryReader.ReadOptInt32(ref byteSpan);
                controlsAllAn = SpanBufferBinaryReader.ReadOptInt32(ref byteSpan);
            }
        }

        private static (int ac, int an, int hc) ReadPopulation(ref ReadOnlySpan<byte> byteSpan)
        {
            int ac = SpanBufferBinaryReader.ReadOptInt32(ref byteSpan);
            int an = SpanBufferBinaryReader.ReadOptInt32(ref byteSpan);
            int hc = SpanBufferBinaryReader.ReadOptInt32(ref byteSpan);
            return (ac, an, hc);
        }

        public GnomadReadEntry Clone() =>
            new GnomadReadEntry
            {
                coverage      = coverage,
                failedFilter  = failedFilter,
                allAc         = allAc,
                allAn         = allAn,
                allHc         = allHc,
                hasAfr        = hasAfr,
                afrAc         = afrAc,
                afrAn         = afrAn,
                afrHc         = afrHc,
                hasAmr        = hasAmr,
                amrAc         = amrAc,
                amrAn         = amrAn,
                amrHc         = amrHc,
                hasAsj        = hasAsj,
                asjAc         = asjAc,
                asjAn         = asjAn,
                asjHc         = asjHc,
                hasEas        = hasEas,
                easAc         = easAc,
                easAn         = easAn,
                easHc         = easHc,
                hasFin        = hasFin,
                finAc         = finAc,
                finAn         = finAn,
                finHc         = finHc,
                hasNfe        = hasNfe,
                nfeAc         = nfeAc,
                nfeAn         = nfeAn,
                nfeHc         = nfeHc,
                hasOth        = hasOth,
                othAc         = afrAc,
                othAn         = othAn,
                othHc         = othHc,
                hasSas        = hasSas,
                sasAc         = sasAc,
                sasAn         = sasAn,
                sasHc         = sasHc,
                hasMale       = hasMale,
                maleAc        = maleAc,
                maleAn        = maleAn,
                maleHc        = maleHc,
                hasFemale     = hasFemale,
                femaleAc      = femaleAc,
                femaleAn      = femaleAn,
                femaleHc      = femaleHc,
                hasControls   = hasControls,
                controlsAllAc = controlsAllAc,
                controlsAllAn = controlsAllAn
            };
    }
}