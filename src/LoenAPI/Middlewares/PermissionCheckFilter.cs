using LoenAPI.Authentication;
using LoenAPI.Extensions;
using Microsoft.AspNetCore.Http;

namespace LoenAPI.Middlewares;

/// <summary>
/// 权限检查过滤器
/// </summary>
/// <param name="permissionService">权限服务</param>
public class PermissionCheckFilter(PermissionService permissionService)
{
    private readonly PermissionService _permissionService = permissionService;

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
        var userId = context.HttpContext.GetUserId();

        if (userId == 0)
        {
            return LoenResponseExtensions.Unauthorized();
        }
        else if (userId > 1)
        {
            var routeName = context.HttpContext.GetEndpoint()?.DisplayName ?? string.Empty;

            if (!string.IsNullOrEmpty(routeName))
            {
                var hasPermission = await _permissionService.CheckPermissionAsync(
                    userId,
                    routeName
                );

                if (!hasPermission)
                {
                    return LoenResponseExtensions.Forbidden();
                }
            }
        }

        return await next(context);
    }
}
