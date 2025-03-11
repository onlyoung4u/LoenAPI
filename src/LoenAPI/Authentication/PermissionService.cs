using LoenAPI.Caching;
using LoenAPI.Models;
using SqlSugar;

namespace LoenAPI.Authentication;

/// <summary>
/// 权限
/// </summary>
public class PermissionService(CacheService cacheService, ISqlSugarClient db)
{
    private readonly CacheService _cacheService = cacheService;
    private readonly ISqlSugarClient _db = db;

    /// <summary>
    /// 获取标识
    /// </summary>
    public async Task<int> GetFlagAsync()
    {
        var flag = await _cacheService.GetMemoryAsync<int>("permission:flag");

        if (flag == 0)
        {
            flag = await RefreshAsync();
        }

        return flag;
    }

    /// <summary>
    /// 刷新
    /// </summary>
    public async Task<int> RefreshAsync()
    {
        var flag = (int)DateTime.Now.Ticks;

        await _cacheService.SetMemoryAsync("permission:flag", flag);

        return flag;
    }

    /// <summary>
    /// 检查权限
    /// </summary>
    public async Task<bool> CheckPermissionAsync(int userId, string permission)
    {
        if (userId == 1)
        {
            return true;
        }

        var permissions = await GetUserPermissionAsync(userId);

        return permissions.Contains(permission);
    }

    /// <summary>
    /// 获取用户权限
    /// </summary>
    public async Task<List<string>> GetUserPermissionAsync(int userId)
    {
        var flag = await GetFlagAsync();
        var cacheKey = $"permission:user:{userId}:{flag}";
        var cacheResult = await _cacheService.GetMemoryAsync<List<string>>(cacheKey);

        if (cacheResult != null)
        {
            return cacheResult;
        }

        var permissions = new List<string>();

        if (userId == 1)
        {
            permissions = await _db.Queryable<LoenMenu>()
                .Select(menu => menu.Permission)
                .ToListAsync();
        }
        else
        {
            var roloIds = await _db.Queryable<LoenUserRole>()
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            if (roloIds.Count == 0)
            {
                await _cacheService.SetMemoryAsync(cacheKey, permissions);

                return permissions;
            }

            permissions = await _db.Queryable<LoenRolePermission>()
                .Where(rp => roloIds.Contains(rp.RoleId))
                .Distinct()
                .Select(rp => rp.Permission)
                .ToListAsync();
        }

        await _cacheService.SetMemoryAsync(cacheKey, permissions);

        return permissions;
    }
}
