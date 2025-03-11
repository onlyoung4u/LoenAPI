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
/// 用户信息
/// </summary>
public class UserInfo
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    public required string Nickname { get; set; }

    /// <summary>
    /// 角色
    /// </summary>
    public required List<string> Roles { get; set; }
}

/// <summary>
/// 修改密码请求
/// </summary>
public class ChangePasswordRequest
{
    /// <summary>
    /// 旧密码
    /// </summary>
    public string OldPassword { get; set; }

    /// <summary>
    /// 新密码
    /// </summary>
    public string NewPassword { get; set; }
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

/// <summary>
/// 修改密码请求验证器
/// </summary>
public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty()
            .WithMessage("旧密码不能为空")
            .MinimumLength(8)
            .WithMessage("旧密码长度不能小于8个字符")
            .MaximumLength(16)
            .WithMessage("旧密码长度不能超过16个字符");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("新密码不能为空")
            .MinimumLength(8)
            .WithMessage("新密码长度不能小于8个字符")
            .MaximumLength(16)
            .WithMessage("新密码长度不能超过16个字符");
    }
}
