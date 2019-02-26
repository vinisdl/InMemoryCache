using System;

namespace InMemoryCache.Caching
{
    public class CacheItem
    {
        private object _cacheValue;

        public CacheItem(object value, DateTime? expiresAt)
        {
            Value = value;
            ExpiresAt = expiresAt;
            LastModifiedTicks = DateTime.UtcNow.Ticks;
        }   

        internal DateTime? ExpiresAt { get; set; }

        internal bool HasExpired => ExpiresAt != null && ExpiresAt < DateTime.UtcNow;

        internal object Value
        {
            get => _cacheValue;
            set
            {
                _cacheValue = value;
                LastModifiedTicks = DateTime.UtcNow.Ticks;
            }
        }

        internal long LastModifiedTicks { get; private set; }
    }
}
