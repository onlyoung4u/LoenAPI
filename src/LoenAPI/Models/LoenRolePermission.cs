using SqlSugar;

namespace LoenAPI.Models;

/// <summary>
/// 角色权限
/// </summary>
[SugarIndex(
    "unique_role_permission",
    nameof(RoleId),
    OrderByType.Asc,
    nameof(Permission),
    OrderByType.Asc,
    true
)]
public class LoenRolePermission : LoenBaseWithoutTimestamp
{
    /// <summary>
    /// 角色 ID
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// 权限
    /// </summary>
    [SugarColumn(Length = 64)]
    public string Permission { get; set; }
}
