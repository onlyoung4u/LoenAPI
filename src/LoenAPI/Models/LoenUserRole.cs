using SqlSugar;

namespace LoenAPI.Models;

/// <summary>
/// 用户角色
/// </summary>
[SugarIndex(
    "unique_user_role",
    nameof(UserId),
    OrderByType.Asc,
    nameof(RoleId),
    OrderByType.Asc,
    true
)]
public class LoenUserRole : LoenBaseWithoutTimestamp
{
    /// <summary>
    /// 用户 ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 角色 ID
    /// </summary>
    public int RoleId { get; set; }
}
