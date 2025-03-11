using LoenAPI.Middlewares;
using LoenAPI.Models;
using LoenAPI.Seeders;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace LoenAPI.Extensions;

/// <summary>
/// 应用程序构建器扩展
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// 使用LoenAPI
    /// </summary>
    /// <param name="app"></param>
    /// <param name="isDevelopment"></param>
    /// <param name="needAdminApi"></param>
    /// <returns></returns>
    public static WebApplication UseLoenAPI(
        this WebApplication app,
        bool isDevelopment = false,
        bool needAdminApi = true
    )
    {
        if (isDevelopment && needAdminApi)
        {
            // 初始化数据库表
            using var scope = app.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();

            db.CodeFirst.InitTables(
                typeof(LoenConfig),
                typeof(LoenMenu),
                typeof(LoenOperationLog),
                typeof(LoenRole),
                typeof(LoenRolePermission),
                typeof(LoenUser),
                typeof(LoenUserRole)
            );

            // 初始化数据库数据
            DatabaseSeeder.Initialize(db);
        }

        // 使用跨域中间件
        app.UseCors("LoenCorsPolicy");

        // 添加异常处理中间件
        app.UseExceptionHandling();

        // 添加基于特性的操作日志中间件
        app.UseAttributeBasedOperationLog();

        // 后台 API
        if (needAdminApi)
        {
            app = app.UseLoenAdminApi();
        }

        return app;
    }
}
