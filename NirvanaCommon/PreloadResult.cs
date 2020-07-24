namespace NirvanaCommon
{
    public class PreloadResult
    {
        public readonly int    Position;
        public readonly string RefAllele;
        public readonly string AltAllele;
        public readonly string Json;

        public PreloadResult(int position, string refAllele, string altAllele, string json)
        {
            Position  = position;
            RefAllele = refAllele;
            AltAllele = altAllele;
            Json      = json;
        }

        public override string ToString() => $"{Position}\t{Json}";
    }
}