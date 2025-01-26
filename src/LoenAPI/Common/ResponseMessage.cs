namespace LoenAPI.Common;

/// <summary>
/// 响应消息
/// </summary>
public static class ResponseMessage
{
    private static readonly Dictionary<ResponseCode, string> Messages = new()
    {
        { ResponseCode.Success, "操作成功" },
        { ResponseCode.Error, "操作失败" },
        { ResponseCode.InvalidParams, "参数错误" },
        { ResponseCode.Unauthorized, "未登录或登录过期" },
        { ResponseCode.Forbidden, "无权限" },
    };

    /// <summary>
    /// 获取消息
    /// </summary>
    /// <param name="code">响应码</param>
    /// <returns>消息</returns>
    public static string GetMessage(ResponseCode code)
    {
        return Messages.TryGetValue(code, out var message) ? message : "未知错误";
    }
}
