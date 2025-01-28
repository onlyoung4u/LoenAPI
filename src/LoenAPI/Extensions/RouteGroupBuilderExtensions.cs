using LoenAPI.Authentication;
using LoenAPI.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace LoenAPI.Extensions;

/// <summary>
/// 路由组构建器扩展
/// </summary>
public static class RouteGroupBuilderExtensions
{
    /// <summary>
    /// 添加JWT认证
    /// </summary>
    /// <param name="group">路由组</param>
    /// <param name="configName">JWT配置名称</param>
    /// <returns>路由组</returns>
    public static RouteGroupBuilder RequireJwtAuth(
        this RouteGroupBuilder group,
        string configName = "Default"
    )
    {
        group.AddEndpointFilter(
            async (context, next) =>
            {
                var jwtService =
                    context.HttpContext.RequestServices.GetRequiredService<IJwtService>();
                var authFilter = new JwtAuthenticationFilter(jwtService, configName);
                return await authFilter.InvokeAsync(context, next);
            }
        );

        return group;
    }

    /// <summary>
    /// 添加权限检查
    /// </summary>
    /// <param name="group">路由组</param>
    /// <returns>路由组</returns>
    public static RouteGroupBuilder RequirePermission(this RouteGroupBuilder group)
    {
        group.AddEndpointFilter(
            async (context, next) =>
            {
                var permissionService =
                    context.HttpContext.RequestServices.GetRequiredService<PermissionService>();
                var permissionFilter = new PermissionCheckFilter(permissionService);
                return await permissionFilter.InvokeAsync(context, next);
            }
        );

        return group;
    }

    /// <summary>
    /// 添加操作日志
    /// </summary>
    /// <param name="group">路由组</param>
    /// <returns>路由组</returns>
    public static RouteGroupBuilder RequireOperationLog(this RouteGroupBuilder group)
    {
        group.AddEndpointFilter(
            async (context, next) =>
            {
                var db = context.HttpContext.RequestServices.GetRequiredService<ISqlSugarClient>();
                var filter = new OperationLogFilter(db);
                return await filter.InvokeAsync(context, next);
            }
        );

        return group;
    }
}
