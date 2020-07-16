using System;

namespace PreloadBaseline.Nirvana
{
    public interface IChromosome : IEquatable<IChromosome>
    {
        int Length { get; }
        ushort Index { get; }
    }
}