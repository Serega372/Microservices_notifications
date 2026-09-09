using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace NotificationsService.Infrastructure.Caching;

public sealed class MemoryCacheStorage
{
    private readonly ILogger<MemoryCacheStorage> _logger;

    private readonly IMemoryCache _memoryCache;

    public MemoryCacheStorage(
        ILogger<MemoryCacheStorage> logger,
        IMemoryCache memoryCache)
    {
        _logger = logger;
        _memoryCache = memoryCache;

        _logger.LogInformation($"{GetType().Name} was initialized");
    }

    public void SetValue<TKey, TValue>(TKey key, TValue value)
    {
        _memoryCache.Set(key, value);
    }
}