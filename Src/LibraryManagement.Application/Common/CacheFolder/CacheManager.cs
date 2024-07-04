using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CacheManager
{
    private readonly IMemoryCache _cache;
    private readonly ConcurrentDictionary<string, HashSet<string>> _cacheTags = new ConcurrentDictionary<string, HashSet<string>>();

    public CacheManager(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> createItem, TimeSpan? absoluteExpirationRelativeToNow = null, string tag = null)
    {
        if (_cache.TryGetValue(key, out T cacheEntry))
        {
            return cacheEntry;
        }

        cacheEntry = await createItem();

        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(5)
        };

        _cache.Set(key, cacheEntry, cacheEntryOptions);

        if (!string.IsNullOrEmpty(tag))
        {
            _cacheTags.AddOrUpdate(tag, new HashSet<string>() { key }, (t, existing) =>
            {
                existing.Add(key);
                return existing;
            });
        }

        return cacheEntry;
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
        foreach (var tag in _cacheTags.Values)
        {
            if (tag.Remove(key)) // check if removal was successful
            {
                var stan = "me";
            }
        }
    }


    public void InvalidateByTag(string tag)
    {
        if (_cacheTags.TryRemove(tag, out var keys))
        {
            foreach (var key in keys)
            {
                Remove(key);
            }
        }
    }

    public void ClearAll()
    {
        foreach (var tag in _cacheTags.Keys)
        {
            InvalidateByTag(tag);
        }
    }

    public IEnumerable<string> GetAllKeys()
    {
        foreach (var tag in _cacheTags.Values)
        {
            foreach (var key in tag)
            {
                yield return key;
            }
        }
    }


}
