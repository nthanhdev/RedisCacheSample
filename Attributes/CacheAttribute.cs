using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RedisCache.Configurations;
using RedisCache.Services;

namespace RedisCache.Attributes
{
    public class CacheAttribute : Attribute, IAsyncActionFilter
    {

        private readonly int _timeToliveSeconds; 

        public CacheAttribute(int timeToLiveSeconds = 1000){
            _timeToliveSeconds = timeToLiveSeconds;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheConfiguration = context.HttpContext.RequestServices.GetRequiredService<RedisConfiguration>();
           
           if (!cacheConfiguration.Enable){
            
                await next();
                return;
           }

           var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>(); 

           var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
           
           var cacheResponse = await cacheService.GetCachedResponseAsync(cacheKey);

           if(!string.IsNullOrEmpty(cacheResponse))
           {
                var contentResult = new ContentResult {

                    Content = cacheResponse , 
                    ContentType = "application/json" , 
                    StatusCode = 200 
                };

                context.Result = contentResult;
                return;
           }

           var excutedContext = await next();
           if(excutedContext.Result is OkObjectResult objectResult)
                await cacheService.SetCacheResponseAsync(cacheKey, objectResult.Value , TimeSpan.FromSeconds(_timeToliveSeconds));
        }

        private static string GenerateCacheKeyFromRequest(HttpRequest httpRequest){

                var keyBuilder = new StringBuilder() ;
                keyBuilder.Append($"{httpRequest.Path}");

                foreach(var (key, value) in httpRequest.Query.OrderBy(p=>p.Key))
                {
                    keyBuilder.Append($"|{key}-{value}");
                }

                return keyBuilder.ToString();
        }
    }
}