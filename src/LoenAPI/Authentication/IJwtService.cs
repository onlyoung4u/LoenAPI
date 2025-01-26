namespace LoenAPI.Authentication;

/// <summary>
/// JWT服务接口
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// 生成Token
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="configName">配置名称</param>
    /// <returns>Token字符串</returns>
    Task<string> GenerateToken(int userId, string configName = "Default");

    /// <summary>
    /// 验证 Token 并返回用户 ID
    /// </summary>
    /// <param name="token">Token字符串</param>
    /// <param name="configName">配置名称</param>
    /// <returns>用户ID</returns>
    Task<int> ValidateToken(string token, string configName = "Default");

    /// <summary>
    /// 注销 Token
    /// </summary>
    /// <param name="token">Token字符串</param>
    /// <param name="configName">配置名称</param>
    Task<bool> BanToken(string token, string configName = "Default");
}
