using System.Collections.Generic;
using Version4.Data;

namespace Version4.Utilities
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