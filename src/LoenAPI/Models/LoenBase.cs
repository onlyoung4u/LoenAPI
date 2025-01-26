using SqlSugar;

namespace LoenAPI.Models;

/// <summary>
/// 基础实体类，包含 id 和时间戳
/// </summary>
public abstract class LoenBase
{
    /// <summary>
    /// 主键
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(InsertServerTime = true)]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(UpdateServerTime = true, IsNullable = true)]
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// 不包含时间戳的基类
/// </summary>
public abstract class LoenBaseWithoutTimestamp
{
    /// <summary>
    /// 主键
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
}
