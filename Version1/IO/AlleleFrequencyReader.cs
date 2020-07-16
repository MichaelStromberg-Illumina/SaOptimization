using System;
using System.IO;
using Version1.Data;

namespace Version1.IO
{
    public class AlleleFrequencyReader : IDisposable
    {
        private readonly Stream _stream;
        private readonly bool _leaveOpen;

        public AlleleFrequencyReader(Stream stream, Index index, bool leaveOpen)
        {
            _stream    = stream;
            _leaveOpen = leaveOpen;
        }

        public void Dispose()
        {
            if (!_leaveOpen) _stream.Dispose();
        }
    }
}