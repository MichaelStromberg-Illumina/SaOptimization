using System.Collections.Generic;
using Version2.Data;

namespace Version2.Utilities
{
    public class BlockComparer : IComparer<WriteBlock>
    {
        public int Compare(WriteBlock a, WriteBlock b)
        {
            if (ReferenceEquals(a,    b)) return 0;
            if (ReferenceEquals(null, b)) return 1;
            return a.Index.CompareTo(b.Index);
        }
    }
}