﻿namespace Compression.Algorithms
{
    public interface ICompressionAlgorithm
    {
        int Decompress(byte[] source, int srcLength, byte[] destination, int destLength);
        int GetCompressedBufferBounds(int srcLength);
    }
}
