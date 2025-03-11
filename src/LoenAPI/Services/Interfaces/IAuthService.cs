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

    /// <summary>
    /// 登出
    /// </summary>
    /// <param name="token"></param>
    Task<bool> Logout(string token);

    /// <summary>
    /// 获取权限
    /// </summary>
    /// <returns></returns>
    Task<List<string>> GetPermissions(int userId);

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <returns></returns>
    Task<UserInfo> GetUserInfo(int userId);

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="request"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task ChangePassword(ChangePasswordRequest request, int userId);
}
