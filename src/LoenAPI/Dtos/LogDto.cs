namespace LoenAPI.Dtos;

/// <summary>
/// 日志请求
/// </summary>
public class LogRequest
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
}
