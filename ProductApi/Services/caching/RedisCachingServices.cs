using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Product_api.Services.caching
{
    public class RedisCachingServices : IRedisCacheService
    {

        private readonly IDistributedCache? _cache;
        public RedisCachingServices(IDistributedCache cache)
        {
            _cache = cache;
        }

        public T? GetData<T>(string key)
        {
            var data = _cache?.GetString(key);

            if (data == null)
                return default(T);

            return JsonSerializer.Deserialize<T>(data);
        }

        public void SetData<T>(string key, T data)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            var jsonData = JsonSerializer.Serialize(data);
            _cache?.SetString(key, jsonData, options);
        }
    }
}
