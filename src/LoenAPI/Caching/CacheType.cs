namespace LoenAPI.Caching;

/// <summary>
/// 缓存类型
/// </summary>
public enum CacheType
{
    /// <summary>
    /// 内存缓存
    /// </summary>
    Memory,

    /// <summary>
    /// Redis缓存
    /// </summary>
    Redis,

    /// <summary>
    /// 两者都使用
    /// </summary>
    Both,
}
