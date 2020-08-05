using System.Collections.Generic;
using Version5.Data;

namespace Version5.Utilities
{
    public class BlockComparer : IComparer<WriteBlock>
    {
        public int Compare(WriteBlock a, WriteBlock b)
        {
            if (ReferenceEquals(a,    b)) return 0;
            return ReferenceEquals(null, b) ? 1 : a.Index.CompareTo(b.Index);
        }
    }
}