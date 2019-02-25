using System;
using System.Collections.Generic;
using System.Text;

namespace InMemoryCache.Interfaces
{
    public interface IInMemoryCache
    {
        bool Del(string key);
        object Get(string key);
        bool Set<T>(string key, T value);
        bool Set<T>(string key, T value, TimeSpan expiresIn);
        int DbSize();
        long Incr(string key);
        long ZAdd(string key, object value, long score);
        long ZCard(string key);
    }
}
