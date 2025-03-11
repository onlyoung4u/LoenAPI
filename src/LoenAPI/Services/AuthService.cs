using LoenAPI.Authentication;
using LoenAPI.Dtos;
using LoenAPI.Exceptions;
using LoenAPI.Models;
using LoenAPI.Services.Interfaces;
using LoenAPI.Utils;
using SqlSugar;

namespace LoenAPI.Services;

/// <summary>
/// 认证服务
/// </summary>
public class AuthService(
    IJwtService jwtService,
    ISqlSugarClient sqlSugarClient,
    PermissionService permissionService
) : IAuthService
{
    private readonly IJwtService _jwtService = jwtService;
    private readonly ISqlSugarClient _sqlSugarClient = sqlSugarClient;
    private readonly PermissionService _permissionService = permissionService;

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="LoenException"></exception>
    public async Task<LoginResponse> Login(LoginRequest request)
    {
        var user = await _sqlSugarClient
            .Queryable<LoenUser>()
            .Where(x => x.Username == request.Username)
            .Where(x => x.IsDeleted == false)
            .FirstAsync();

        if (user == null)
        {
            throw new LoenException("用户名或密码错误");
        }

        if (!BcryptUtil.VerifyPassword(request.Password, user.Password))
        {
            throw new LoenException("用户名或密码错误");
        }

        if (!user.IsActive)
        {
            throw new LoenException("用户已被禁用");
        }

        try
        {
            var token = await _jwtService.GenerateToken(user.Id);

            return new LoginResponse { Token = token };
        }
        catch (Exception)
        {
            throw new LoenException("登录失败");
        }
    }

    /// <summary>
    /// 登出
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<bool> Logout(string token)
    {
        var result = await _jwtService.BanToken(token);

        return result;
    }

    /// <summary>
    /// 获取权限
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<string>> GetPermissions(int userId)
    {
        return await _permissionService.GetUserPermissionAsync(userId);
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<UserInfo> GetUserInfo(int userId)
    {
        var user = await _sqlSugarClient
            .Queryable<LoenUser>()
            .Includes(x => x.Roles)
            .InSingleAsync(userId);

        var roles = userId == 1 ? ["超级管理员"] : user.Roles.Select(x => x.Name).ToList();

        return new UserInfo
        {
            Id = user.Id,
            Username = user.Username,
            Nickname = user.Nickname,
            Roles = roles,
        };
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="request"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task ChangePassword(ChangePasswordRequest request, int userId)
    {
        var user = await _sqlSugarClient.Queryable<LoenUser>().InSingleAsync(userId);

        if (!BcryptUtil.VerifyPassword(request.OldPassword, user.Password))
        {
            throw new LoenException("旧密码错误");
        }

        await _sqlSugarClient
            .Updateable<LoenUser>()
            .SetColumns(x => x.Password == BcryptUtil.HashPassword(request.NewPassword))
            .Where(x => x.Id == userId)
            .ExecuteCommandAsync();
    }
}
