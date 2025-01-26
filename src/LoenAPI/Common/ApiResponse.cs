namespace LoenAPI.Common;

/// <summary>
/// API 响应
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="code">响应码</param>
/// <param name="message">消息</param>
/// <param name="data">数据</param>
public class ApiResponse<T>(
    ResponseCode code = ResponseCode.Success,
    string? message = null,
    T? data = default
)
{
    /// <summary>
    /// 响应码
    /// </summary>
    public int Code { get; set; } = (int)code;

    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get; set; } = message ?? ResponseMessage.GetMessage(code);

    /// <summary>
    /// 数据
    /// </summary>
    public T? Data { get; set; } = data;
}

/// <summary>
/// API 响应
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="code">响应码</param>
/// <param name="message">消息</param>
public class ApiResponse(ResponseCode code = ResponseCode.Success, string? message = null)
    : ApiResponse<object>(code, message) { }
