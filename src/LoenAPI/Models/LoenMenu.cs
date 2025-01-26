using SqlSugar;

namespace LoenAPI.Models;

/// <summary>
/// 菜单
/// </summary>
[SugarIndex("unique_menu_permission", nameof(Permission), OrderByType.Asc, true)]
public class LoenMenu : LoenBase
{
    /// <summary>
    /// 父菜单 ID
    /// </summary>
    public int ParentId { get; set; }

    /// <summary>
    /// 菜单名
    /// </summary>
    [SugarColumn(Length = 64)]
    public string Title { get; set; }

    /// <summary>
    /// 菜单路径
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// 菜单权限
    /// </summary>
    public string Permission { get; set; }

    /// <summary>
    /// 菜单图标
    /// </summary>
    [SugarColumn(Length = 64)]
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// 菜单链接
    /// </summary>
    public string Link { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 是否隐藏
    /// </summary>
    public bool Hidden { get; set; } = false;

    /// <summary>
    /// 是否为系统菜单
    /// </summary>
    public bool IsSystem { get; set; } = false;
}
