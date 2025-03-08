using Microsoft.AspNetCore.Http;

namespace LoenAPI.Extensions;

/// <summary>
/// HttpContext 扩展
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// 获取用户ID
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <returns>用户ID</returns>
    public static int GetUserId(this HttpContext context)
    {
        return context.Items["UserId"] is int userId ? userId : 0;
    }

    /// <summary>
    /// 获取真实 IP
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <returns>真实 IP</returns>
    public static string GetRealIp(this HttpContext context)
    {
        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();

        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
    }
}
