using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using InMemoryCache.Interfaces;
using static System.Int64;

namespace InMemoryCache.Caching
{
    public class InMemoryCache : IInMemoryCache
    {
        private readonly ConcurrentDictionary<string, CacheItem> _memory;

        public InMemoryCache()
        {
            _memory = new ConcurrentDictionary<string, CacheItem>();
        }

        private bool TryGetValue(string key, out CacheItem item)
        {
            return _memory.TryGetValue(key, out item);
        }

        private void Set(string key, CacheItem item)
        {
            _memory[key] = item;
        }

        private bool CacheSet(string key, object value, DateTime? expiresAt = null)
        {
            if (!TryGetValue(key, out var entry))
            {
                entry = new CacheItem(value, expiresAt);
                Set(key, entry);
                return true;
            }

            lock (entry)
            {
                entry.Value = value;
                entry.ExpiresAt = expiresAt;
            }

            return true;
        }

        private bool CacheScoreSet(string key, object value, long score)
        {
            if (!TryGetValue(key, out var entry))
            {
                var itensOfScoreSet = new List<SortedCacheItem>();
                itensOfScoreSet.Add(new SortedCacheItem(value, score));
                CacheSet(key, itensOfScoreSet);
                return true;
            }

            lock (entry)
            {
                var sortedItens = entry.Value as List<SortedCacheItem>;
                var sortedItem = sortedItens.FirstOrDefault(item => item.Value.Equals(value));
                if (sortedItem == null)
                    sortedItens.Add(new SortedCacheItem(value, score));
                else
                    sortedItem.Score = score;

                return true;
            }
        }

        #region Public

        public bool Del(string key)
        {
            return _memory.TryRemove(key, out _);
        }

        public object Get(string key)
        {
            if (!_memory.TryGetValue(key, out var cacheEntry)) return null;
            if (!cacheEntry.HasExpired) return cacheEntry.Value;

            _memory.TryRemove(key, out cacheEntry);
            return null;

        }

        public bool Set<T>(string key, T value)
        {
            return CacheSet(key, value);
        }

        public bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            return CacheSet(key, value, DateTime.UtcNow.Add(expiresIn));
        }

        public int DbSize() => _memory.Count;


        public long Incr(string key)
        {
            if (_memory.TryGetValue(key, out var cacheItem))
            {
                lock (cacheItem)
                {
                    if (TryParse(cacheItem.Value.ToString(), out var value))
                    {
                        cacheItem.Value = ++value;
                        return value;
                    }
                }
            }

            Set(key, 0);
            return Incr(key);
        }


        public long ZAdd(string key, object value, long score)
        {
            CacheScoreSet(key, value, score);
            return score;
        }

        public long ZCard(string key)
        {
            if (TryGetSortedItens(key, out var collection))
                return 0;
            var entry = collection as List<SortedCacheItem>;
            return entry?.Count ?? 0;
        }

        public long? ZRank(string key, object value)
        {
            if (TryGetSortedItens(key, out var collection))
                return null;

            return collection.FindIndex(sorted => sorted.Value.Equals(value));
        }

        private bool TryGetSortedItens(string key, out List<SortedCacheItem> entry)
        {
            entry = null;
            var collection = Get(key);
            if (collection == null)
                return true;

            entry = collection as List<SortedCacheItem>;
            return false;
        }

        public IList<T> ZRange<T>(string key, int start, int stop)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}