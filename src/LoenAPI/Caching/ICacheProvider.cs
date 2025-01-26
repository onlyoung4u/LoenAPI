namespace LoenAPI.Caching;

/// <summary>
/// 缓存提供器接口
/// </summary>
public interface ICacheProvider
{
    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <typeparam name="T">返回数据类型</typeparam>
    /// <param name="key">缓存键</param>
    /// <returns>缓存的数据</returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="key">缓存键</param>
    /// <param name="value">缓存值</param>
    /// <param name="expiration">过期时间</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>
    /// 移除缓存
    /// </summary>
    /// <param name="key">缓存键</param>
    Task RemoveAsync(string key);

    /// <summary>
    /// 判断缓存是否存在
    /// </summary>
    /// <param name="key">缓存键</param>
    /// <returns>是否存在</returns>
    Task<bool> ExistsAsync(string key);
}
