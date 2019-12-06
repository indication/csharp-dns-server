// // //------------------------------------------------------------------------------------------------- 
// // // <copyright file="DnsCache.cs" company="stephbu">
// // // Copyright (c) Steve Butler. All rights reserved.
// // // </copyright>
// // //-------------------------------------------------------------------------------------------------

namespace Dns
{
    using System;
    using Microsoft.Extensions.Caching.Memory;
    using Dns.Contracts;

    public class DnsCache<T> : IDnsCache<T>
    {
        private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        T IDnsCache<T>.Get(string key)
        {
            T data;
            if (_cache.TryGetValue(key, out data)) {
                return data;
            }

            return default(T);
        }

        void IDnsCache<T>.Set(string key, T data, int ttlSeconds)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTimeOffset.Now + TimeSpan.FromSeconds(ttlSeconds));
            _cache.Set(key, data, cacheEntryOptions);
        }
    }
}