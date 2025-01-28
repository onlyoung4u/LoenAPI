using FluentValidation;
using LoenAPI.Models;

namespace LoenAPI.Dtos;

public class MenuMeta
{
    public required string Title { get; set; }
    public string? Icon { get; set; }
    public string? Link { get; set; }
}

public class MenusDto
{
    public required string Name { get; set; }
    public required string Path { get; set; }
    public required string Component { get; set; }
    public required MenuMeta Meta { get; set; }
    public string? Redirect { get; set; }
    public List<MenusDto>? Children { get; set; }
}

public class MenuListDto
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public string Title { get; set; }
    public string Path { get; set; }
    public string Permission { get; set; }
    public string Icon { get; set; }
    public string Link { get; set; }
    public int Sort { get; set; }
    public bool Hidden { get; set; }
    public bool IsSystem { get; set; }
    public List<MenuListDto>? Children { get; set; }
}

public class LoenMenuRequestDto
{
    public int? ParentId { get; set; }
    public string? Title { get; set; }
    public string? Path { get; set; }
    public string? Permission { get; set; }
    public string? Icon { get; set; }
    public string? Link { get; set; }
    public int? Sort { get; set; }
    public bool? Hidden { get; set; }
}

public class LoenMenuResponseValidator : AbstractValidator<LoenMenuRequestDto>
{
    public LoenMenuResponseValidator()
    {
        RuleFor(x => x.ParentId).NotNull().GreaterThanOrEqualTo(0).WithMessage("未知的父级菜单");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("标题不能为空")
            .Length(1, 64)
            .WithMessage("标题长度不能超过64个字符");

        RuleFor(x => x.Path)
            .NotNull()
            .Must(x => string.IsNullOrEmpty(x) || x.StartsWith('/'))
            .WithMessage("路径必须以 / 开头")
            .MaximumLength(64)
            .WithMessage("路径长度不能超过64个字符");

        RuleFor(x => x.Permission)
            .NotEmpty()
            .WithMessage("权限不能为空")
            .MaximumLength(64)
            .WithMessage("权限长度不能超过64个字符");

        RuleFor(x => x.Icon).NotNull().MaximumLength(64).WithMessage("图标长度不能超过64个字符");

        RuleFor(x => x.Link)
            .NotNull()
            .Must(x => x == string.Empty || Uri.TryCreate(x, UriKind.Absolute, out _))
            .WithMessage("链接必须是有效的URL")
            .MaximumLength(255)
            .WithMessage("链接长度不能超过255个字符");

        RuleFor(x => x.Sort)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .WithMessage("排序必须大于等于0")
            .LessThanOrEqualTo(255)
            .WithMessage("排序不能超过255");
    }
}
