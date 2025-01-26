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

public class MenuListDto : LoenMenu
{
    public List<LoenMenu>? Children { get; set; }
}

public class LoenMenuRequestDto
{
    public int ParentId { get; set; }
    public string Title { get; set; }
    public string Path { get; set; }
    public string Permission { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public int Sort { get; set; } = 0;
    public bool Hidden { get; set; } = false;
}

public class LoenMenuResponseValidator : AbstractValidator<LoenMenuRequestDto>
{
    public LoenMenuResponseValidator()
    {
        RuleFor(x => x.ParentId)
            .NotEmpty()
            .WithMessage("父级菜单不能为空")
            .GreaterThanOrEqualTo(0)
            .WithMessage("错误的父级菜单");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("标题不能为空")
            .Length(1, 64)
            .WithMessage("标题长度不能超过64个字符");

        RuleFor(x => x.Path)
            .NotEmpty()
            .WithMessage("路径不能为空")
            .Must(x => x.StartsWith('/'))
            .WithMessage("路径必须以 / 开头")
            .Length(1, 64)
            .WithMessage("路径长度不能超过64个字符");

        RuleFor(x => x.Permission)
            .NotEmpty()
            .WithMessage("权限不能为空")
            .Length(1, 64)
            .WithMessage("权限长度不能超过64个字符");

        RuleFor(x => x.Icon).MaximumLength(64).WithMessage("图标长度不能超过64个字符");

        RuleFor(x => x.Link)
            .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _))
            .WithMessage("链接必须是有效的URL")
            .MaximumLength(255)
            .WithMessage("链接长度不能超过255个字符");

        RuleFor(x => x.Sort)
            .GreaterThanOrEqualTo(0)
            .WithMessage("排序必须大于等于0")
            .LessThanOrEqualTo(255)
            .WithMessage("排序不能超过255");
    }
}
