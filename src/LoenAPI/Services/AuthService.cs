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
public class AuthService(IJwtService jwtService, ISqlSugarClient sqlSugarClient) : IAuthService
{
    private readonly IJwtService _jwtService = jwtService;
    private readonly ISqlSugarClient _sqlSugarClient = sqlSugarClient;

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
}
