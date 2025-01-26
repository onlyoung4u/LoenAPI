using SqlSugar;

namespace LoenAPI.Models;

/// <summary>
/// 配置
/// </summary>
[SugarIndex("unique_config_key", nameof(Key), OrderByType.Asc, true)]
public class LoenConfig : LoenBase
{
    /// <summary>
    /// 配置分组
    /// </summary>
    public string Group { get; set; }

    /// <summary>
    /// 配置键
    /// </summary>
    [SugarColumn(Length = 64)]
    public string Key { get; set; }

    /// <summary>
    /// 配置名称
    /// </summary>
    [SugarColumn(Length = 64)]
    public string Name { get; set; }

    /// <summary>
    /// 配置值
    /// </summary>
    [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string Value { get; set; }

    /// <summary>
    /// 配置选项
    /// </summary>
    [SugarColumn(IsJson = true, IsNullable = true)]
    public string Options { get; set; }

    /// <summary>
    /// 配置备注
    /// </summary>
    [SugarColumn(Length = 255)]
    public string Remark { get; set; } = string.Empty;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsActive { get; set; } = true;
}
