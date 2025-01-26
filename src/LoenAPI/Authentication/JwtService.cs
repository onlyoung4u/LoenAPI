using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LoenAPI.Caching;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LoenAPI.Authentication;

/// <summary>
/// JWT服务
/// </summary>
public class JwtService : IJwtService
{
    private readonly Dictionary<string, JwtConfig> _jwtConfigs;
    private readonly CacheService _cacheService;

    private class JwtConfig
    {
        public JwtOptions Options { get; set; }
        public SigningCredentials Credentials { get; set; }

        public JwtConfig(JwtOptions options)
        {
            Options = options;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey));
            Credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options"></param>
    /// <param name="cacheService"></param>
    /// <exception cref="ArgumentException"></exception>
    public JwtService(IOptions<List<JwtOptions>> options, CacheService cacheService)
    {
        _jwtConfigs = options.Value.ToDictionary(opt => opt.Name, opt => new JwtConfig(opt));

        // 确保至少有一个默认配置
        if (!_jwtConfigs.ContainsKey("Default"))
        {
            throw new ArgumentException("必须提供一个名为 'Default' 的JWT配置");
        }

        _cacheService = cacheService;
    }

    private static string GetCacheKey(int userId)
    {
        return $"loen:token:{userId}";
    }

    private static string GetCacheKey(string token)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(token));
        return $"loen:token:blacklist:{Convert.ToHexStringLower(hash)}";
    }

    /// <summary>
    /// 生成Token
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="configName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<string> GenerateToken(int userId, string configName = "Default")
    {
        if (!_jwtConfigs.TryGetValue(configName, out var config))
        {
            throw new ArgumentException($"未找到名为 '{configName}' 的JWT配置");
        }

        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) };

        if (config.Options.SSO)
        {
            var uuid = Guid.NewGuid().ToString("N");

            claims.Add(new Claim("sso", uuid));

            await _cacheService.SetRedisAsync(
                GetCacheKey(userId),
                uuid,
                TimeSpan.FromMinutes(config.Options.ExpiresInMinutes)
            );
        }

        var token = new JwtSecurityToken(
            issuer: config.Options.Issuer,
            audience: config.Options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(config.Options.ExpiresInMinutes),
            signingCredentials: config.Credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// 验证Token
    /// </summary>
    /// <param name="token"></param>
    /// <param name="configName"></param>
    /// <returns>用户ID</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<int> ValidateToken(string token, string configName = "Default")
    {
        if (!_jwtConfigs.TryGetValue(configName, out var config))
        {
            throw new ArgumentException($"未找到名为 '{configName}' 的JWT配置");
        }

        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Options.SecretKey));

            var principal = new JwtSecurityTokenHandler().ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = config.Options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = config.Options.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                },
                out SecurityToken validatedToken
            );

            var claims = principal.Claims;

            var userId = int.Parse(
                claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "0"
            );

            if (userId == 0)
            {
                return 0;
            }

            var isBlacklisted = await _cacheService.ExistsRedisAsync(GetCacheKey(token));

            if (isBlacklisted)
            {
                return 0;
            }

            if (config.Options.SSO)
            {
                var sso = claims.FirstOrDefault(c => c.Type == "sso")?.Value;

                if (sso == null)
                {
                    return 0;
                }

                var cacheValue = await _cacheService.GetRedisAsync<string>(GetCacheKey(userId));

                if (!string.Equals(cacheValue, sso))
                {
                    return 0;
                }
            }

            return userId;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// 注销 Token
    /// </summary>
    /// <param name="token"></param>
    /// <param name="configName"></param>
    /// <returns>是否成功</returns>
    public async Task<bool> BanToken(string token, string configName = "Default")
    {
        try
        {
            var claims = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims;

            var expires = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;

            if (expires == null)
            {
                return false;
            }

            var expiration = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expires)).UtcDateTime;

            var userId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            await _cacheService.SetRedisAsync(
                GetCacheKey(token),
                userId,
                expiration - DateTime.UtcNow
            );

            return true;
        }
        catch
        {
            return false;
        }
    }
}
