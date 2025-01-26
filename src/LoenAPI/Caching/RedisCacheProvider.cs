using System.Text.Json;
using StackExchange.Redis;

namespace LoenAPI.Caching;

/// <summary>
/// Redis缓存提供器
/// </summary>
public class RedisCacheProvider : ICacheProvider
{
    private readonly IConnectionMultiplexer _redis;
    private IDatabase Database => _redis.GetDatabase();

    public RedisCacheProvider(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await Database.StringGetAsync(key);
        if (!value.HasValue)
            return default;

        return JsonSerializer.Deserialize<T>(value!);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        return Database.StringSetAsync(key, serializedValue, expiration);
    }

    public Task RemoveAsync(string key)
    {
        return Database.KeyDeleteAsync(key);
    }

    public Task<bool> ExistsAsync(string key)
    {
        return Database.KeyExistsAsync(key);
    }
}
