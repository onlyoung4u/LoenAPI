namespace LoenAPI.Common;

/// <summary>
/// 响应码
/// </summary>
public enum ResponseCode
{
    /// <summary>
    /// 成功
    /// </summary>
    Success = 0,

    /// <summary>
    /// 错误
    /// </summary>
    Error = 1,

    /// <summary>
    /// 参数错误
    /// </summary>
    InvalidParams = 2,

    /// <summary>
    /// 未授权
    /// </summary>
    Unauthorized = 1000,

    /// <summary>
    /// 禁止访问
    /// </summary>
    Forbidden = 1001,
}
