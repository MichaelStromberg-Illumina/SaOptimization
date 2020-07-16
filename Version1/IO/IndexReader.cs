using System;
using System.IO;

namespace Version1.IO
{
    public class IndexReader: IDisposable
    {
        private readonly Stream _stream;
        private readonly bool _leaveOpen;

        public IndexReader(Stream stream, bool leaveOpen)
        {
            _stream    = stream;
            _leaveOpen = leaveOpen;
        }

        public Data.Index Read()
        {
            throw new NotImplementedException();
            return null;
        }
        
        public void Dispose()
        {
            if (!_leaveOpen) _stream.Dispose();
        }
    }
}