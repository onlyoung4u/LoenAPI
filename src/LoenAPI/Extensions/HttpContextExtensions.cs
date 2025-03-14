using System.Collections.Concurrent;
using System.Reflection;
using LoenAPI.Dtos;
using Microsoft.AspNetCore.Http;

namespace LoenAPI.Extensions;

/// <summary>
/// HttpContext 扩展
/// </summary>
public static class HttpContextExtensions
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new();

    /// <summary>
    /// 获取用户ID
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <returns>用户ID</returns>
    public static int GetUserId(this HttpContext context)
    {
        return context.Items["UserId"] is int userId ? userId : 0;
    }

    /// <summary>
    /// 获取Token
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <returns>Token</returns>
    public static string GetToken(this HttpContext context)
    {
        var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

        return token ?? string.Empty;
    }

    /// <summary>
    /// 获取真实 IP
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <returns>真实 IP</returns>
    public static string GetRealIp(this HttpContext context)
    {
        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();

        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// 获取分页请求参数
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <returns>分页请求参数</returns>
    public static PaginationRequest GetPaginationRequest(this HttpContext context)
    {
        var pagination = new PaginationRequest();

        if (
            context.Request.Query.TryGetValue("page", out var page)
            && int.TryParse(page, out var pageValue)
        )
        {
            if (pageValue > 0)
            {
                pagination.Page = pageValue;
            }
        }

        if (
            context.Request.Query.TryGetValue("limit", out var limit)
            && int.TryParse(limit, out var limitValue)
        )
        {
            if (limitValue > 0 && limitValue <= 100)
            {
                pagination.Limit = limitValue;
            }
        }

        return pagination;
    }

    /// <summary>
    /// 安全获取查询参数
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="context">HttpContext</param>
    /// <param name="key">参数名</param>
    /// <returns>参数值，如果参数不存在或格式不正确则返回默认值</returns>
    public static T? GetQueryParam<T>(this HttpContext context, string key)
        where T : struct
    {
        if (!context.Request.Query.TryGetValue(key, out var value) || string.IsNullOrEmpty(value))
        {
            return null;
        }

        try
        {
            if (typeof(T) == typeof(int))
            {
                if (int.TryParse(value, out var intValue))
                {
                    return (T)(object)intValue;
                }
            }
            else if (typeof(T) == typeof(DateTime))
            {
                if (DateTime.TryParse(value, out var dateTimeValue))
                {
                    return (T)(object)dateTimeValue;
                }
            }
            else if (typeof(T) == typeof(bool))
            {
                if (bool.TryParse(value, out var boolValue))
                {
                    return (T)(object)boolValue;
                }
            }
            else if (typeof(T) == typeof(double))
            {
                if (double.TryParse(value, out var doubleValue))
                {
                    return (T)(object)doubleValue;
                }
            }
            else if (typeof(T) == typeof(decimal))
            {
                if (decimal.TryParse(value, out var decimalValue))
                {
                    return (T)(object)decimalValue;
                }
            }
            else if (typeof(T) == typeof(string))
            {
                return (T)(object)value;
            }
        }
        catch
        {
            // 忽略解析异常
        }

        return null;
    }

    /// <summary>
    /// 安全获取查询参数字符串
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <param name="key">参数名</param>
    /// <returns>参数值，如果参数不存在则返回 null</returns>
    public static string? GetQueryParamString(this HttpContext context, string key)
    {
        if (context.Request.Query.TryGetValue(key, out var value) && !string.IsNullOrEmpty(value))
        {
            return value.ToString();
        }

        return null;
    }

    /// <summary>
    /// 获取查询参数并映射到指定类型
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="context">HttpContext</param>
    /// <returns>映射后的对象</returns>
    public static T GetQueryParams<T>(this HttpContext context)
        where T : class, new()
    {
        var result = new T();

        var properties = PropertyCache.GetOrAdd(typeof(T), t => t.GetProperties());

        foreach (var property in properties)
        {
            // 获取属性类型
            var propertyType = property.PropertyType;

            if (
                context.Request.Query.TryGetValue(property.Name, out var value)
                && !string.IsNullOrEmpty(value)
            )
            {
                // 处理可空类型
                if (
                    propertyType.IsGenericType
                    && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                )
                {
                    propertyType = Nullable.GetUnderlyingType(propertyType);
                }

                if (propertyType == typeof(string))
                {
                    property.SetValue(result, value);
                }
                else if (propertyType == typeof(int))
                {
                    if (int.TryParse(value, out var intValue))
                    {
                        property.SetValue(result, intValue);
                    }
                }
                else if (propertyType == typeof(DateTime))
                {
                    if (DateTime.TryParse(value, out var dateTimeValue))
                    {
                        property.SetValue(result, dateTimeValue);
                    }
                }
                else if (propertyType == typeof(bool))
                {
                    if (bool.TryParse(value, out var boolValue))
                    {
                        property.SetValue(result, boolValue);
                    }
                }
                else if (propertyType == typeof(double))
                {
                    if (double.TryParse(value, out var doubleValue))
                    {
                        property.SetValue(result, doubleValue);
                    }
                }
                else if (propertyType == typeof(decimal))
                {
                    if (decimal.TryParse(value, out var decimalValue))
                    {
                        property.SetValue(result, decimalValue);
                    }
                }
            }
        }

        return result;
    }
}
