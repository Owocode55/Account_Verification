using Com.Xpresspayments.AVS.Data.Model;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.Common.Utiities
{
    public static class CacheUtility
    {
        public static void SetAppDetailsCache(IMemoryCache memoryCache, List<Client> client, string key, int cacheMinute)
        {
            var cacheExpiryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(cacheMinute),
                Priority = Microsoft.Extensions.Caching.Memory.CacheItemPriority.High,
                SlidingExpiration = TimeSpan.FromMinutes(5),
                Size = 1024,
            };
            memoryCache.Set(key, client, cacheExpiryOptions);
            
        }

        public static List<Client> GetAppDetailsCache(IMemoryCache memoryCache , string key)
        {
            List<Client> client = null;
            memoryCache.TryGetValue(key, out client);
            return client;
        }


        public static void SetProviderCache(IMemoryCache memoryCache, List<Provider> client, string key , int cacheMinute)
        {
            var cacheExpiryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(cacheMinute),
                Priority = Microsoft.Extensions.Caching.Memory.CacheItemPriority.High,
                SlidingExpiration = TimeSpan.FromMinutes(5),
                Size = 1024,
            };
            memoryCache.Set(key, client, cacheExpiryOptions);

        }

        public static List<Provider> GetProviderCache(IMemoryCache memoryCache, string key)
        {
            List<Provider> providers = null;
            memoryCache.TryGetValue(key, out providers);
            return providers;
        }


        public static void RemoveCache(IMemoryCache memoryCache, string key)
        {
            memoryCache.Remove(key);
        }
    }
}
