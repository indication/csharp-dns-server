// // //------------------------------------------------------------------------------------------------- 
// // // <copyright file="IDnsCache.cs" company="stephbu">
// // // Copyright (c) Steve Butler. All rights reserved.
// // // </copyright>
// // //-------------------------------------------------------------------------------------------------

namespace Dns.Contracts
{
    public interface IDnsCache<T>
    {
        T Get(string key);

        void Set(string key, T data, int ttlSeconds);
    }
}