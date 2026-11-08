namespace NotificationsService.Application.Interfaces;

public interface IMemoryCacheStorage
{
    void SetValue<TKey, TValue>(TKey key, TValue value, TimeSpan expiration);

    void SetValue<TKey, TValue>(TKey key, TValue value);

    TValue? GetValue<TKey, TValue>(TKey key) where TValue : class;
}