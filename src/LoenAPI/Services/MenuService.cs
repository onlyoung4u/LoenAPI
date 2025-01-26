using LoenAPI.Authentication;
using LoenAPI.Dtos;
using LoenAPI.Models;
using LoenAPI.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using SqlSugar;

namespace LoenAPI.Services;

public class MenuService(ISqlSugarClient db, PermissionService permissionService) : IMenuService
{
    private readonly ISqlSugarClient _db = db;
    private readonly PermissionService _permissionService = permissionService;

    private static MenusDto HandleMenu(LoenMenu menu, string path)
    {
        var fullPath = path + menu.Path;
        var component = path.IsNullOrEmpty() ? "BasicLayout" : fullPath;
        var meta = new MenuMeta { Title = menu.Title };

        if (!menu.Icon.IsNullOrEmpty())
        {
            meta.Icon = menu.Icon;
        }

        if (!menu.Link.IsNullOrEmpty())
        {
            meta.Link = menu.Link;
        }

        return new MenusDto
        {
            Name = menu.Permission,
            Path = fullPath,
            Component = component,
            Meta = meta,
        };
    }

    private static List<MenusDto> HandleMenuTree(
        List<LoenMenu> menus,
        int parentId = 0,
        string path = ""
    )
    {
        var tree = new List<MenusDto>();

        foreach (var menu in menus)
        {
            if (menu.ParentId == parentId)
            {
                var menusDto = HandleMenu(menu, path);

                var children = HandleMenuTree(menus, menu.Id, path + menu.Path);

                if (children.Count > 0)
                {
                    menusDto.Children = children;
                }

                tree.Add(menusDto);
            }
        }

        return tree;
    }

    public async Task<List<MenusDto>> Menus(int userId)
    {
        var menus = new List<LoenMenu>();

        if (userId == 1)
        {
            menus = await _db.Queryable<LoenMenu>()
                .Where(x => x.Hidden == false)
                .OrderBy(x => x.Sort)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }
        else
        {
            var permissions = await _permissionService.GetUserPermissionAsync(userId);

            if (permissions.Count > 0)
            {
                menus = await _db.Queryable<LoenMenu>()
                    .Where(x => x.Hidden == false)
                    .Where(x => permissions.Contains(x.Permission))
                    .OrderBy(x => x.Sort)
                    .OrderBy(x => x.Id)
                    .ToListAsync();
            }
        }

        return HandleMenuTree(menus);
    }
}
