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

        adminGroup
            .MapPost(
                "/logout",
                async (HttpContext context, IAuthService authService) =>
                {
                    var result = await authService.Logout(context.GetToken());

                    return LoenResponseExtensions.SuccessOrError(result);
                }
            )
            .WithName("logout")
            .WithMetadata(new LogOperationAttribute("登出"));

        adminGroup.MapGet(
            "/menus",
            async (HttpContext context, IMenuService menuService) =>
            {
                var userId = context.GetUserId();
                var menus = await menuService.Menus(userId);
                return LoenResponseExtensions.Success(menus);
            }
        );

        adminGroup.MapGet(
            "/permissions",
            async (HttpContext context, IAuthService authService) =>
            {
                var userId = context.GetUserId();
                var permissions = await authService.GetPermissions(userId);
                return LoenResponseExtensions.Success(permissions);
            }
        );

        adminGroup.MapGet(
            "/user/info",
            async (HttpContext context, IAuthService authService) =>
            {
                var userId = context.GetUserId();
                var userInfo = await authService.GetUserInfo(userId);
                return LoenResponseExtensions.Success(userInfo);
            }
        );

        adminGroup
            .MapPost(
                "/change-password",
                async (
                    HttpContext context,
                    IAuthService authService,
                    IValidationService validationService,
                    IValidator<ChangePasswordRequest> validator,
                    ChangePasswordRequest? request
                ) =>
                {
                    var validatedRequest = await validationService.ValidateAsync(
                        request,
                        validator
                    );
                    await authService.ChangePassword(validatedRequest, context.GetUserId());
                    return LoenResponseExtensions.Success();
                }
            )
            .WithName("change-password")
            .WithMetadata(new LogOperationAttribute("修改密码"));

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
                "/menu/{id:int:min(1)}",
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
                "/menu/{id:int:min(1)}",
                async (IMenuService menuService, int id) =>
                {
                    await menuService.MenuDelete(id);
                    return LoenResponseExtensions.Success();
                }
            )
            .WithName("menu.delete")
            .WithMetadata(new LogOperationAttribute("删除菜单"));

        adminGroup
            .MapGet(
                "/logs",
                async (HttpContext context, ILogService logService) =>
                {
                    var paginationResponse = await logService.GetLogs(
                        context.GetPaginationRequest(),
                        context.GetQueryParams<LogRequest>()
                    );

                    return LoenResponseExtensions.Success(paginationResponse);
                }
            )
            .WithName("logs");

        return app;
    }
}
