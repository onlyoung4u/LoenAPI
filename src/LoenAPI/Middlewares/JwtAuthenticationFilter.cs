using LoenAPI.Authentication;
using LoenAPI.Extensions;
using Microsoft.AspNetCore.Http;

namespace LoenAPI.Middlewares;

/// <summary>
/// JWT认证过滤器
/// </summary>
/// <param name="jwtService">JWT服务</param>
/// <param name="configName">JWT配置名称</param>
public class JwtAuthenticationFilter(IJwtService jwtService, string configName = "Default")
{
    private readonly IJwtService _jwtService = jwtService;
    private readonly string _configName = configName;

    /// <summary>
    /// 处理请求
    /// </summary>
    /// <param name="context">请求上下文</param>
    /// <param name="next">下一个过滤器</param>
    /// <returns>处理结果</returns>
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        var token = context.HttpContext.GetToken();

        if (string.IsNullOrEmpty(token))
        {
            return LoenResponseExtensions.Unauthorized();
        }

        try
        {
            var userId = await _jwtService.ValidateToken(token, _configName);

            if (userId == 0)
            {
                return LoenResponseExtensions.Unauthorized();
            }

            context.HttpContext.Items["UserId"] = userId;
        }
        catch
        {
            return LoenResponseExtensions.Unauthorized();
        }

        return await next(context);
    }
}
