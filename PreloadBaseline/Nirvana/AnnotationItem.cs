namespace PreloadBaseline.Nirvana
{
    public sealed class AnnotationItem
    {
        public readonly int Position;
        public readonly byte[] Data;

        public AnnotationItem(int position, byte[] data)
        {
            Position = position;
            Data     = data;
        }
    }
}