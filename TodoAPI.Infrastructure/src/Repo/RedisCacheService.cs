using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TodoAPI.Application.Repo;

namespace TodoAPI.Infrastructure
{
    internal sealed class RedisCacheService : ICacheService
    {
        public RedisCacheService(IDistributedCache cache, ILogger<ICacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        private readonly IDistributedCache _cache;
        private readonly ILogger _logger;

        public async Task DeleteFromCacheAsync(string cachedKey, CancellationToken ct)
        {
            try
            {
                await _cache.RemoveAsync(cachedKey, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при удалении ключа {CacheKey} из Redis", cachedKey);
            }
        }

        public async Task<T?> ReadFromCacheAsync<T>(string cachedKey, CancellationToken ct)
        {
            try
            {
                var cachedData = await _cache.GetStringAsync(cachedKey, ct);

                if (string.IsNullOrEmpty(cachedData)) return default;

                var cachedItem = JsonSerializer.Deserialize<T>(cachedData);
                return cachedItem;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при чтении ключа {CacheKey} из Redis. Запрос пойдет в БД.", cachedKey);
                return default;
            }
        }

        public async Task WriteInCacheAsync<T>(string cachedKey, T cacheItem, CancellationToken ct, 
            CacheDuration absExp = CacheDuration.FiveMinutes, 
            CacheSlidingDuration slidingExp = CacheSlidingDuration.TwoMintues)
        {
            try
            {
                var ttl = TimeSpan.FromMinutes((int)absExp);
                var ttlContinuation = TimeSpan.FromMinutes((int)slidingExp);

                await _cache.SetStringAsync(
                    cachedKey,
                    JsonSerializer.Serialize(cacheItem),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = ttl,
                        SlidingExpiration = ttlContinuation
                    },
                    ct);
            }
            catch(Exception ex) 
            {
                _logger.LogWarning(ex, "Ошибка при записи ключа {CacheKey} в Redis", cachedKey);
            }
        }

        public async Task WriteInCacheAsync<T>(string cachedKey, T cacheItem, CancellationToken ct, TimeSpan absExp)
        {
            try
            {
                await _cache.SetStringAsync(
                    cachedKey,
                    JsonSerializer.Serialize(cacheItem),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = absExp,
                    },
                    ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при записи ключа {CacheKey} в Redis", cachedKey);
            }
        }
    }
}
