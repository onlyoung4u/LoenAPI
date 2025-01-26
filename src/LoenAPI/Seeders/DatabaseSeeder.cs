using LoenAPI.Models;
using LoenAPI.Utils;
using SqlSugar;

namespace LoenAPI.Seeders;

/// <summary>
/// 数据库种子
/// </summary>
public class DatabaseSeeder
{
    private const string Create = "create";
    private const string Update = "update";
    private const string Delete = "delete";
    private const string Handle = "handle";

    private static readonly string[] AllActions = [Create, Update, Delete, Handle];

    private static readonly Dictionary<string, string> ActionLabels = new()
    {
        { Create, "添加" },
        { Update, "修改" },
        { Delete, "删除" },
        { Handle, "处理" },
    };

    /// <summary>
    /// 初始化数据库
    /// </summary>
    /// <param name="db"></param>
    public static void Initialize(ISqlSugarClient db)
    {
        SeedAdmin(db);
        SeedMenu(db);
    }

    private static void SeedAdmin(ISqlSugarClient db)
    {
        var adminIsExist = db.Queryable<LoenUser>().Any(u => u.Id == 1);

        if (adminIsExist)
        {
            return;
        }

        var admin = new LoenUser
        {
            Username = "admin",
            Nickname = "超级管理员",
            Password = BcryptUtil.HashPassword("lucky@loen"),
        };

        db.Insertable(admin).ExecuteCommand();
    }

    private static List<LoenMenu> GetActions(int parentId, string perfix, string[]? actions = null)
    {
        actions ??= [Create, Update, Delete];

        var menus = new List<LoenMenu>();

        foreach (var action in actions)
        {
            if (!AllActions.Contains(action))
            {
                continue;
            }

            menus.Add(
                new LoenMenu
                {
                    ParentId = parentId,
                    Title = ActionLabels[action],
                    Path = "",
                    Permission = perfix + "." + action,
                    Hidden = true,
                    IsSystem = true,
                }
            );
        }

        return menus;
    }

    private static void SeedMenu(ISqlSugarClient db)
    {
        var menuIsExist = db.Queryable<LoenMenu>().Any(m => m.Id == 1);

        if (menuIsExist)
        {
            return;
        }

        // 系统设置-菜单
        var systemMenu = new LoenMenu
        {
            ParentId = 0,
            Title = "系统管理",
            Path = "/system",
            Permission = "system",
            Icon = "material-symbols:settings",
            Sort = 99,
            IsSystem = true,
        };

        var systemMenuId = db.Insertable(systemMenu).ExecuteReturnIdentity();

        // 系统设置-用户管理
        var userMenu = new LoenMenu
        {
            ParentId = systemMenuId,
            Title = "用户管理",
            Path = "/users",
            Permission = "user.list",
            IsSystem = true,
        };

        var userMenuId = db.Insertable(userMenu).ExecuteReturnIdentity();

        db.Insertable(GetActions(userMenuId, "user")).ExecuteCommand();

        // 系统设置-角色管理
        var roleMenu = new LoenMenu
        {
            ParentId = systemMenuId,
            Title = "角色管理",
            Path = "/roles",
            Permission = "role.list",
            IsSystem = true,
        };

        var roleMenuId = db.Insertable(roleMenu).ExecuteReturnIdentity();

        db.Insertable(GetActions(roleMenuId, "role")).ExecuteCommand();

        // 系统设置-菜单管理
        var menuMenu = new LoenMenu
        {
            ParentId = systemMenuId,
            Title = "菜单管理",
            Path = "/menus",
            Permission = "menu.list",
            IsSystem = true,
        };

        var menuMenuId = db.Insertable(menuMenu).ExecuteReturnIdentity();

        db.Insertable(GetActions(menuMenuId, "menu")).ExecuteCommand();

        // 系统设置-系统配置
        var settingsMenu = new LoenMenu
        {
            ParentId = systemMenuId,
            Title = "系统配置",
            Path = "/settings",
            Permission = "settings",
            IsSystem = true,
        };

        db.Insertable(settingsMenu).ExecuteCommand();

        // 系统设置-配置管理
        var configMenu = new LoenMenu
        {
            ParentId = systemMenuId,
            Title = "配置管理",
            Path = "/configs",
            Permission = "config.list",
            IsSystem = true,
        };

        var configMenuId = db.Insertable(configMenu).ExecuteReturnIdentity();

        db.Insertable(GetActions(configMenuId, "config")).ExecuteCommand();

        // 系统设置-操作日志
        var logMenu = new LoenMenu
        {
            ParentId = systemMenuId,
            Title = "操作日志",
            Path = "/logs",
            Permission = "logs",
            IsSystem = true,
        };

        db.Insertable(logMenu).ExecuteCommand();
    }
}
