using FluentValidation;
using LoenAPI.Dtos;
using LoenAPI.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace LoenAPI.Extensions;

/// <summary>
/// 应用程序构建器扩展
/// </summary>
public static class LoenAdminApiExtensions
{
    /// <summary>
    /// 使用 Loen Admin API
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseLoenAdminApi(this WebApplication app)
    {
        app.MapPost(
            "/admin/login",
            async (
                IAuthService authService,
                IValidationService validationService,
                IValidator<LoginRequest> validator,
                LoginRequest? request
            ) =>
            {
                var validatedRequest = await validationService.ValidateAsync(request, validator);

                var result = await authService.Login(validatedRequest);

                return LoenResponseExtensions.Success(result);
            }
        );

        var adminGroup = app.MapGroup("/admin").RequireJwtAuth().RequirePermission();

        adminGroup.MapGet(
            "/menus",
            async (HttpContext context, IMenuService menuService) =>
            {
                var userId = context.GetUserId();

                var menus = await menuService.Menus(userId);

                return LoenResponseExtensions.Success(menus);
            }
        );

        return app;
    }
}
