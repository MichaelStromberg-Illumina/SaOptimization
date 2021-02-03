namespace Version6.Data
{
    public class PreloadResult
    {
        public readonly ulong  PositionAllele;
        public readonly string Json;

        public PreloadResult(ulong positionAllele, string json)
        {
            PositionAllele = positionAllele;
            Json           = json;
        }
    }
}