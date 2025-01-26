using LoenAPI.Dtos;

namespace LoenAPI.Services.Interfaces;

/// <summary>
/// 认证服务
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<LoginResponse> Login(LoginRequest request);
}
