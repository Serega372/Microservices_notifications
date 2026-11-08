using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NotificationsService.Application.Interfaces;

namespace NotificationsService.Infrastructure.Caching;

public sealed class MemoryCacheStorage : IMemoryCacheStorage
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

    public void SetValue<TKey, TValue>(TKey key, TValue value, TimeSpan expiration)
    {
        _memoryCache.Set((typeof(TValue), key), value, expiration);
    }

    public void SetValue<TKey, TValue>(TKey key, TValue value)
    {
        _memoryCache.Set((typeof(TValue), key), value);
    }

    public TValue? GetValue<TKey, TValue>(TKey key) where TValue : class
    {
        if (!_memoryCache.TryGetValue((typeof(TValue), key), out var value))
        {
            _logger.LogInformation($"Notr found active valid session by key {key}");
            return null;
        }

        if (value is not TValue resultValue)
        {
            _logger.LogError($"Value by key {key} miss own type");
            return null;
        }

        return resultValue;
    }
}