using LoenAPI.Dtos;
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
    /// 获取Token
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <returns>Token</returns>
    public static string GetToken(this HttpContext context)
    {
        var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

        return token ?? string.Empty;
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

    /// <summary>
    /// 获取分页请求参数
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <returns>分页请求参数</returns>
    public static PaginationRequest GetPaginationRequest(this HttpContext context)
    {
        var pagination = new PaginationRequest();

        if (
            context.Request.Query.TryGetValue("page", out var page)
            && int.TryParse(page, out var pageValue)
        )
        {
            if (pageValue > 0)
            {
                pagination.Page = pageValue;
            }
        }

        if (
            context.Request.Query.TryGetValue("limit", out var limit)
            && int.TryParse(limit, out var limitValue)
        )
        {
            if (limitValue > 0 && limitValue <= 100)
            {
                pagination.Limit = limitValue;
            }
        }

        return pagination;
    }
}
