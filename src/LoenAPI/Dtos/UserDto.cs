using FluentValidation;

namespace LoenAPI.Dtos;

/// <summary>
/// 登录请求
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }
}

/// <summary>
/// 登录响应
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// 令牌
    /// </summary>
    public required string Token { get; set; }
}

/// <summary>
/// 登录请求验证器
/// </summary>
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("用户名不能为空")
            .Length(1, 64)
            .WithMessage("用户名长度不能超过64个字符");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("密码不能为空")
            .Length(1, 64)
            .WithMessage("密码长度不能超过64个字符");
    }
}
