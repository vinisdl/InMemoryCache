using System;
using System.Collections.Generic;
using System.Text;

namespace InMemoryCache.Caching
{
    public class SortedCacheItem
    {
        internal object Value { get; set; }
        internal long Score { get; set; }

        public SortedCacheItem(object value, long score)
        {
            Score = score;
            Value = value;
        }
    }
}
