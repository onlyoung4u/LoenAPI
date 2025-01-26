namespace LoenAPI.Utils;

/// <summary>
/// Bcrypt 密码加密工具类
/// </summary>
public static class BcryptUtil
{
    /// <summary>
    /// 加密密码
    /// </summary>
    /// <param name="password">明文密码</param>
    /// <returns>加密后的密码</returns>
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
    }

    /// <summary>
    /// 验证密码
    /// </summary>
    /// <param name="password">明文密码</param>
    /// <param name="hashedPassword">加密后的密码</param>
    /// <returns>是否匹配</returns>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
