using LoenAPI.Common;

namespace LoenAPI.Exceptions;

/// <summary>
/// 业务异常基类
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="message">消息</param>
/// <param name="code">响应码</param>
public class LoenException(string message, ResponseCode code = ResponseCode.Error)
    : Exception(message)
{
    /// <summary>
    /// 响应码
    /// </summary>
    public ResponseCode Code { get; } = code;
}

/// <summary>
/// 未授权异常
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="message">消息</param>
public class LoenUnauthorizedException(string? message = null)
    : LoenException(
        message ?? ResponseMessage.GetMessage(ResponseCode.Unauthorized),
        ResponseCode.Unauthorized
    ) { }

/// <summary>
/// 禁止访问异常
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="message">消息</param>
public class LoenForbiddenException(string? message = null)
    : LoenException(
        message ?? ResponseMessage.GetMessage(ResponseCode.Forbidden),
        ResponseCode.Forbidden
    ) { }

/// <summary>
/// 参数验证异常
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="message">消息</param>
public class LoenValidationException(string? message = null)
    : LoenException(
        message ?? ResponseMessage.GetMessage(ResponseCode.InvalidParams),
        ResponseCode.InvalidParams
    ) { }
