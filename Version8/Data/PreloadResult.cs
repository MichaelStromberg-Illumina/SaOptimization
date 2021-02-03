namespace Version8.Data
{
    public class PreloadResult
    {
        public readonly ulong  PositionAllele;
        public readonly string RsId;

        public PreloadResult(ulong positionAllele, string rsId)
        {
            PositionAllele = positionAllele;
            RsId           = rsId;
        }
    }
}