using LoenAPI.Common;
using LoenAPI.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace LoenAPI.Middlewares;

/// <summary>
/// 异常处理中间件
/// </summary>
public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger
)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    /// <summary>
    /// 处理请求
    /// </summary>
    /// <param name="context">Http 上下文</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// 处理异常
    /// </summary>
    /// <param name="context">Http 上下文</param>
    /// <param name="exception">异常</param>
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.Headers.Append("X-Loen-Error", "true");

        var response = exception switch
        {
            LoenException ex => new ApiResponse(ex.Code, ex.Message),
            SqlSugarException => new ApiResponse(ResponseCode.Error),
            _ => new ApiResponse(ResponseCode.Error, "服务器内部错误"),
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}

/// <summary>
/// 扩展方法
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    /// <summary>
    /// 使用异常处理中间件
    /// </summary>
    /// <param name="builder">IApplicationBuilder</param>
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
