namespace LoenAPI.Caching;

/// <summary>
/// 缓存服务
/// </summary>
public class CacheService(MemoryCacheProvider memoryCache, RedisCacheProvider redisCache)
{
    private readonly MemoryCacheProvider _memoryCache = memoryCache;
    private readonly RedisCacheProvider _redisCache = redisCache;

    /// <summary>
    /// 获取缓存
    /// </summary>
    public async Task<T?> GetAsync<T>(string key, CacheType type = CacheType.Both)
    {
        if (type == CacheType.Memory || type == CacheType.Both)
        {
            var memoryResult = await _memoryCache.GetAsync<T>(key);
            if (memoryResult != null)
                return memoryResult;
        }

        if (type == CacheType.Redis || type == CacheType.Both)
        {
            var redisResult = await _redisCache.GetAsync<T>(key);
            if (redisResult != null && type == CacheType.Both)
            {
                await _memoryCache.SetAsync(key, redisResult);
            }
            return redisResult;
        }

        return default;
    }

    /// <summary>
    /// 获取内存缓存
    /// </summary>
    public async Task<T?> GetMemoryAsync<T>(string key)
    {
        return await GetAsync<T>(key, CacheType.Memory);
    }

    /// <summary>
    /// 获取Redis缓存
    /// </summary>
    public async Task<T?> GetRedisAsync<T>(string key)
    {
        return await GetAsync<T>(key, CacheType.Redis);
    }

    /// <summary>
    /// 设置缓存
    /// </summary>
    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CacheType type = CacheType.Both
    )
    {
        if (type == CacheType.Memory || type == CacheType.Both)
        {
            await _memoryCache.SetAsync(key, value, expiration);
        }

        if (type == CacheType.Redis || type == CacheType.Both)
        {
            await _redisCache.SetAsync(key, value, expiration);
        }
    }

    /// <summary>
    /// 设置内存缓存
    /// </summary>
    public async Task SetMemoryAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        await SetAsync(key, value, expiration, CacheType.Memory);
    }

    /// <summary>
    /// 设置Redis缓存
    /// </summary>
    public async Task SetRedisAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        await SetAsync(key, value, expiration, CacheType.Redis);
    }

    /// <summary>
    /// 移除缓存
    /// </summary>
    public async Task RemoveAsync(string key, CacheType type = CacheType.Both)
    {
        if (type == CacheType.Memory || type == CacheType.Both)
        {
            await _memoryCache.RemoveAsync(key);
        }

        if (type == CacheType.Redis || type == CacheType.Both)
        {
            await _redisCache.RemoveAsync(key);
        }
    }

    /// <summary>
    /// 移除内存缓存
    /// </summary>
    public async Task RemoveMemoryAsync(string key)
    {
        await RemoveAsync(key, CacheType.Memory);
    }

    /// <summary>
    /// 移除Redis缓存
    /// </summary>
    public async Task RemoveRedisAsync(string key)
    {
        await RemoveAsync(key, CacheType.Redis);
    }

    /// <summary>
    /// 判断缓存是否存在
    /// </summary>
    public async Task<bool> ExistsAsync(string key, CacheType type = CacheType.Both)
    {
        if (type == CacheType.Memory || type == CacheType.Both)
        {
            if (await _memoryCache.ExistsAsync(key))
                return true;
        }

        if (type == CacheType.Redis || type == CacheType.Both)
        {
            return await _redisCache.ExistsAsync(key);
        }

        return false;
    }

    /// <summary>
    /// 判断内存缓存是否存在
    /// </summary>
    public async Task<bool> ExistsMemoryAsync(string key)
    {
        return await ExistsAsync(key, CacheType.Memory);
    }

    /// <summary>
    /// 判断Redis缓存是否存在
    /// </summary>
    public async Task<bool> ExistsRedisAsync(string key)
    {
        return await ExistsAsync(key, CacheType.Redis);
    }
}
