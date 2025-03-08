using LoenAPI.Authentication;
using LoenAPI.Dtos;
using LoenAPI.Exceptions;
using LoenAPI.Models;
using LoenAPI.Services.Interfaces;
using Mapster;
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

        var mainMenu = new MenusDto
        {
            Name = "index",
            Path = "/index",
            Component = "BasicLayout",
            Meta = new MenuMeta { Title = "首页", Icon = "lucide:layout-dashboard" },
            Redirect = "/dashboard",
            Children =
            [
                new MenusDto
                {
                    Name = "dashboard",
                    Path = "/dashboard",
                    Component = "/dashboard/workspace/index",
                    Meta = new MenuMeta
                    {
                        Title = "首页",
                        HideInMenu = true,
                        ActivePath = "/index",
                    },
                },
            ],
        };

        var tree = HandleMenuTree(menus);

        tree.Insert(0, mainMenu);

        return tree;
    }

    private static List<MenuListDto> HandleMenuList(List<LoenMenu> menus, int parentId = 0)
    {
        var tree = new List<MenuListDto>();

        foreach (var menu in menus)
        {
            if (menu.ParentId == parentId)
            {
                var menuListDto = menu.Adapt<MenuListDto>();

                var children = HandleMenuList(menus, menu.Id);

                if (children.Count > 0)
                {
                    menuListDto.Children = children;
                }

                tree.Add(menuListDto);
            }
        }

        return tree;
    }

    public async Task<List<MenuListDto>> MenuList()
    {
        var menus = await _db.Queryable<LoenMenu>()
            .OrderBy(x => x.Sort)
            .OrderBy(x => x.Id)
            .ToListAsync();

        return HandleMenuList(menus);
    }

    public async Task MenuCreate(LoenMenuRequestDto menu)
    {
        if (menu.ParentId > 0)
        {
            var parentMenu = await _db.Queryable<LoenMenu>().AnyAsync(x => x.Id == menu.ParentId);

            if (!parentMenu)
            {
                throw new LoenException("父级菜单不存在");
            }
        }

        if (!menu.Path.IsNullOrEmpty())
        {
            var samePath = await _db.Queryable<LoenMenu>()
                .Where(x => x.ParentId == menu.ParentId)
                .Where(x => x.Path == menu.Path)
                .AnyAsync();

            if (samePath)
            {
                throw new LoenException("菜单路径已存在");
            }
        }

        var samePermission = await _db.Queryable<LoenMenu>()
            .Where(x => x.Permission == menu.Permission)
            .AnyAsync();

        if (samePermission)
        {
            throw new LoenException("菜单权限已存在");
        }

        var menuEntity = menu.Adapt<LoenMenu>();

        await _db.Insertable(menuEntity).ExecuteCommandAsync();
    }

    public async Task MenuUpdate(int id, LoenMenuRequestDto menu)
    {
        var menuEntity = await _db.Queryable<LoenMenu>().Where(x => x.Id == id).FirstAsync();

        if (menuEntity == null)
        {
            throw new LoenException("菜单不存在");
        }

        if (menuEntity.IsSystem)
        {
            throw new LoenException("系统菜单不能修改");
        }

        if (menu.ParentId > 0)
        {
            if (menu.ParentId == id)
            {
                throw new LoenException("父级菜单不能是自身");
            }

            var parentMenu = await _db.Queryable<LoenMenu>().AnyAsync(x => x.Id == menu.ParentId);

            if (!parentMenu)
            {
                throw new LoenException("父级菜单不存在");
            }
        }

        if (!menu.Path.IsNullOrEmpty())
        {
            var samePath = await _db.Queryable<LoenMenu>()
                .Where(x => x.ParentId == menu.ParentId)
                .Where(x => x.Path == menu.Path)
                .Where(x => x.Id != id)
                .AnyAsync();

            if (samePath)
            {
                throw new LoenException("菜单路径已存在");
            }
        }

        var samePermission = await _db.Queryable<LoenMenu>()
            .Where(x => x.Permission == menu.Permission)
            .Where(x => x.Id != id)
            .AnyAsync();

        if (samePermission)
        {
            throw new LoenException("菜单权限已存在");
        }

        menuEntity = menu.Adapt(menuEntity);

        await _db.Updateable(menuEntity).ExecuteCommandAsync();
    }

    public async Task<int[]> GetAllChildrenId(int id, HashSet<int>? visited = null)
    {
        visited ??= [];

        if (visited.Contains(id))
        {
            return [];
        }

        visited.Add(id);

        var menus = await _db.Queryable<LoenMenu>().Where(x => x.ParentId == id).ToListAsync();

        var ids = menus.Select(x => x.Id).ToArray();

        foreach (var menu in menus)
        {
            ids = [.. ids, .. await GetAllChildrenId(menu.Id, visited)];
        }

        return ids;
    }

    public async Task MenuDelete(int id)
    {
        var menu = await _db.Queryable<LoenMenu>().Where(x => x.Id == id).FirstAsync();

        if (menu == null)
        {
            throw new LoenException("菜单不存在");
        }

        if (menu.IsSystem)
        {
            throw new LoenException("系统菜单不能删除");
        }

        int[] ids = [id, .. await GetAllChildrenId(id)];

        var permissions = await _db.Queryable<LoenMenu>()
            .Where(x => ids.Contains(x.Id))
            .Select(x => x.Permission)
            .ToListAsync();

        var result = _db.Ado.UseTran(() =>
        {
            _db.Deleteable<LoenMenu>().Where(x => ids.Contains(x.Id)).ExecuteCommand();

            _db.Deleteable<LoenRolePermission>()
                .Where(x => permissions.Contains(x.Permission))
                .ExecuteCommand();

            return true;
        });

        if (result.Data)
        {
            await _permissionService.RefreshAsync();
        }
    }
}
