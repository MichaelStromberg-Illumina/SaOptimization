using System.Collections.Generic;
using Version1.Data;

namespace Version1.Utilities
{
    public static class FrequencyUtilities
    {
        public static (TsvEntry[] CommonEntries, TsvEntry[] RareEntries) GroupVariants(List<TsvEntry> entries)
        {
            var commonEntries = new List<TsvEntry>();
            var rareEntries   = new List<TsvEntry>();

            foreach (TsvEntry entry in entries)
            {
                if (entry.IsCommon) commonEntries.Add(entry);
                else rareEntries.Add(entry);
            }

            return (commonEntries.ToArray(), rareEntries.ToArray());
        }
    }
}