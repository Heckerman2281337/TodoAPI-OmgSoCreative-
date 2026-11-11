namespace TodoAPI.Application.Repo
{
    public enum CacheDuration
    {
        FiveMinutes = 5,
        FifteenMinutes = 15
    }
    public enum CacheSlidingDuration
    { 
        TwoMintues = 2,
        FiveMinutes = 5
    }

    public interface ICacheService
    {
        public Task WriteInCacheAsync<T>(string cachedKey, T cacheItem, CancellationToken ct,
            CacheDuration absExp = CacheDuration.FiveMinutes, 
            CacheSlidingDuration slidingExp = CacheSlidingDuration.TwoMintues);
        public Task WriteInCacheAsync<T>(string cachedKey, T cacheItem, CancellationToken ct,
            TimeSpan absExp);
        public Task<T?> ReadFromCacheAsync<T>(string cachedKey, CancellationToken ct);
        public Task DeleteFromCacheAsync(string cachedKey, CancellationToken ct);
    }
}
