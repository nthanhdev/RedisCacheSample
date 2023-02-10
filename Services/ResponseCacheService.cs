using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace RedisCache.Services
{
    public class ResponseCacheService : IResponseCacheService
    {

        private readonly IDistributedCache _distributedCache;

        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public ResponseCacheService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer)
        {
            _distributedCache = distributedCache;
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task<string?> GetCachedResponseAsync(string cacheKey)
        {
    

            var cacheResponse = await _distributedCache.GetStringAsync(cacheKey);

            return cacheResponse ??= null;

        }

        public async Task RemoveCacheResponseAsync(string pattern)
        {
            if(string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentException("Value cannot be null or whitespace"); 

            await foreach (var key in GetKeysAsync(pattern + "*"))
            {
                await _distributedCache.RemoveAsync(key);
            }
        }

        private async IAsyncEnumerable<string> GetKeysAsync(string pattern)
        {
             if(string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentException("Value cannot be null or whitespace"); 
             foreach (var endPoint in _connectionMultiplexer.GetEndPoints())
             {
                var server = _connectionMultiplexer.GetServer(endPoint);
                foreach (var key in server.Keys(pattern: pattern))
                {
                    yield return key.ToString();
                }
             }
        }
        public async Task SetCacheResponseAsync(string cacheKey, object? response, TimeSpan timeOut)
        {
           // kiểm tra response 
            if (response == null)
                return;

            var serializerResponse = JsonConvert.SerializeObject(response , new JsonSerializerSettings(){
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            await _distributedCache.SetStringAsync(cacheKey , serializerResponse , new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = timeOut
            });
            
        }
        
    }
}