using SqlSugar;

namespace LoenAPI.Models;

/// <summary>
/// 角色
/// </summary>
public class LoenRole : LoenBase
{
    /// <summary>
    /// 角色名
    /// </summary>
    [SugarColumn(Length = 64)]
    public string Name { get; set; }

    /// <summary>
    /// 创建者 ID
    /// </summary>
    public int CreatorId { get; set; }

    /// <summary>
    /// 创建者
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(CreatorId))]
    public LoenUser Creator { get; set; }
}
