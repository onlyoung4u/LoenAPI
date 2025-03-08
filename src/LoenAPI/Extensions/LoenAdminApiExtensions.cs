using FluentValidation;
using LoenAPI.Attributes;
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
                    var validatedRequest = await validationService.ValidateAsync(
                        request,
                        validator
                    );
                    var result = await authService.Login(validatedRequest);
                    return LoenResponseExtensions.Success(result);
                }
            )
            .WithName("login")
            .WithMetadata(new LogOperationAttribute("登录"));

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

        adminGroup
            .MapGet(
                "/menu",
                async (IMenuService menuService) =>
                {
                    var menus = await menuService.MenuList();
                    return LoenResponseExtensions.Success(menus);
                }
            )
            .WithName("menu.list");

        adminGroup
            .MapPost(
                "/menu",
                async (
                    IMenuService menuService,
                    IValidationService validationService,
                    IValidator<LoenMenuRequestDto> validator,
                    LoenMenuRequestDto? menu
                ) =>
                {
                    var validatedMenu = await validationService.ValidateAsync(menu, validator);
                    await menuService.MenuCreate(validatedMenu);
                    return LoenResponseExtensions.Success();
                }
            )
            .WithName("menu.create")
            .WithMetadata(new LogOperationAttribute("创建菜单"));

        adminGroup
            .MapPut(
                "/menu/{id}",
                async (
                    IMenuService menuService,
                    IValidationService validationService,
                    IValidator<LoenMenuRequestDto> validator,
                    int id,
                    LoenMenuRequestDto? menu
                ) =>
                {
                    var validatedMenu = await validationService.ValidateAsync(menu, validator);
                    await menuService.MenuUpdate(id, validatedMenu);
                    return LoenResponseExtensions.Success();
                }
            )
            .WithName("menu.update")
            .WithMetadata(new LogOperationAttribute("修改菜单"));

        adminGroup
            .MapDelete(
                "/menu/{id}",
                async (IMenuService menuService, int id) =>
                {
                    await menuService.MenuDelete(id);
                    return LoenResponseExtensions.Success();
                }
            )
            .WithName("menu.delete")
            .WithMetadata(new LogOperationAttribute("删除菜单"));

        return app;
    }
}
