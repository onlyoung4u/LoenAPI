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
}
