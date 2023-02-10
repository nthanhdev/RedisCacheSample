using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisCache.Services
{
    public interface IResponseCacheService
    {
        Task SetCacheResponseAsync(string cacheKey, object? response, TimeSpan timeOut);

        Task<string?> GetCachedResponseAsync(string cacheKey);

        Task RemoveCacheResponseAsync(string partern);
    }
}