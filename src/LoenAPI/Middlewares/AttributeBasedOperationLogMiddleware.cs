using System.Collections.Concurrent;
using System.Text;
using LoenAPI.Attributes;
using LoenAPI.Extensions;
using LoenAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace LoenAPI.Middlewares;

/// <summary>
/// 基于特性的操作日志中间件
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="next">请求委托</param>
/// <param name="db">数据库客户端</param>
/// <param name="logger">日志记录器</param>
public class AttributeBasedOperationLogMiddleware(
    RequestDelegate next,
    ISqlSugarClient db,
    ILogger<AttributeBasedOperationLogMiddleware> logger
)
{
    private readonly RequestDelegate _next = next;
    private readonly ISqlSugarClient _db = db;
    private readonly ILogger<AttributeBasedOperationLogMiddleware> _logger = logger;

    private static readonly ConcurrentDictionary<
        int,
        (string Username, string Nickname)
    > _userCache = new();

    /// <summary>
    /// 处理请求
    /// </summary>
    /// <param name="context">Http上下文</param>
    public async Task InvokeAsync(HttpContext context)
    {
        // 获取端点
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            await _next(context);
            return;
        }

        // 检查端点是否应用了 LogOperationAttribute 特性
        var logAttribute = endpoint.Metadata.GetMetadata<LogOperationAttribute>();
        if (logAttribute == null)
        {
            await _next(context);
            return;
        }

        // 获取路由名称
        var routeName = endpoint.Metadata.GetMetadata<EndpointNameMetadata>()?.EndpointName;

        // 获取用户信息
        var userId = context.GetUserId();
        var (username, nickname) = await GetUserInfoAsync(userId);

        // 确保请求体可以被多次读取
        context.Request.EnableBuffering();

        // 读取请求体
        var requestBody = string.Empty;
        if (logAttribute.LogRequestBody && context.Request.ContentLength > 0)
        {
            using var reader = new StreamReader(
                context.Request.Body,
                Encoding.UTF8,
                leaveOpen: true
            );
            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        var log = new LoenOperationLog
        {
            UserId = userId,
            Username = username,
            Nickname = nickname,
            Path = context.Request.GetDisplayUrl(),
            Route = routeName ?? string.Empty,
            Method = context.Request.Method,
            Ip = context.GetRealIp(),
            Body = requestBody,
            Success = false,
            Description = logAttribute.Description,
        };

        try
        {
            // 执行下一个中间件
            await _next(context);

            // 判断是否成功
            if (
                context.Response.StatusCode == StatusCodes.Status200OK
                && !context.Response.Headers.ContainsKey("X-Loen-Error")
            )
            {
                log.Success = true;
            }
        }
        catch
        {
            throw;
        }
        finally
        {
            try
            {
                await _db.Insertable(log).ExecuteCommandAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "写入操作日志失败");
            }
        }
    }

    // 获取用户信息，优先从缓存获取
    private async Task<(string Username, string Nickname)> GetUserInfoAsync(int userId)
    {
        if (userId <= 0)
        {
            return (string.Empty, string.Empty);
        }

        // 尝试从缓存获取
        if (_userCache.TryGetValue(userId, out var userInfo))
        {
            return userInfo;
        }

        // 缓存未命中，从数据库查询
        var user = await _db.Queryable<LoenUser>().Where(x => x.Id == userId).FirstAsync();

        if (user == null)
        {
            return (string.Empty, string.Empty);
        }

        // 更新缓存
        var result = (user.Username, user.Nickname);
        _userCache[userId] = result;

        return result;
    }
}

/// <summary>
/// 基于特性的操作日志中间件扩展
/// </summary>
public static class AttributeBasedOperationLogMiddlewareExtensions
{
    /// <summary>
    /// 使用基于特性的操作日志中间件
    /// </summary>
    /// <param name="builder">应用程序构建器</param>
    /// <returns>应用程序构建器</returns>
    public static IApplicationBuilder UseAttributeBasedOperationLog(
        this IApplicationBuilder builder
    )
    {
        return builder.UseMiddleware<AttributeBasedOperationLogMiddleware>();
    }
}
