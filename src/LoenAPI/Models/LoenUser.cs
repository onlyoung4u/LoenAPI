using SqlSugar;

namespace LoenAPI.Models;

/// <summary>
/// 用户
/// </summary>
[SugarIndex("unique_user_username", nameof(Username), OrderByType.Asc, true)]
public class LoenUser : LoenBase
{
    /// <summary>
    /// 用户名
    /// </summary>
    [SugarColumn(Length = 64)]
    public string Username { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    [SugarColumn(Length = 64)]
    public string Nickname { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 最后登录 IP
    /// </summary>
    [SugarColumn(Length = 39)]
    public string LastLoginIp { get; set; } = string.Empty;

    /// <summary>
    /// 最后登录时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 是否删除
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// 用户角色
    /// </summary>
    [Navigate(typeof(LoenUserRole), nameof(LoenUserRole.UserId), nameof(LoenUserRole.RoleId))]
    public List<LoenRole> Roles { get; set; }
}
