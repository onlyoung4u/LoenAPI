using SqlSugar;

namespace LoenAPI.Models;

/// <summary>
/// 操作日志
/// </summary>
public class LoenOperationLog : LoenBase
{
    /// <summary>
    /// 用户 ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    [SugarColumn(Length = 64)]
    public string Username { get; set; }

    /// <summary>
    /// 用户昵称
    /// </summary>
    [SugarColumn(Length = 64)]
    public string Nickname { get; set; }

    /// <summary>
    /// 请求路径
    /// </summary>
    [SugarColumn(Length = 255)]
    public string Path { get; set; }

    /// <summary>
    /// 请求路由
    /// </summary>
    [SugarColumn(Length = 255)]
    public string Route { get; set; }

    /// <summary>
    /// 请求方法
    /// </summary>
    [SugarColumn(Length = 10)]
    public string Method { get; set; }

    /// <summary>
    /// 请求 IP
    /// </summary>
    [SugarColumn(Length = 39)]
    public string Ip { get; set; }

    /// <summary>
    /// 请求体
    /// </summary>
    [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string Body { get; set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 操作描述
    /// </summary>
    [SugarColumn(Length = 255)]
    public string Description { get; set; }
}
