using System.Collections.Concurrent;
using System.Linq.Expressions;
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

    private static readonly ConcurrentDictionary<string, Delegate> PropertySetterCache = new();

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
        var type = typeof(T);

        // 使用缓存获取属性信息
        var properties = PropertyCache.GetOrAdd(type, t => t.GetProperties());

        foreach (var property in properties)
        {
            var propertyName = property.Name;

            // 如果查询参数中不存在该属性名，则跳过
            if (
                !context.Request.Query.TryGetValue(propertyName, out var value)
                || string.IsNullOrEmpty(value)
            )
            {
                continue;
            }

            // 获取属性类型
            var propertyType = property.PropertyType;
            var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            // 尝试转换并设置属性值
            object? convertedValue = null;

            try
            {
                // 根据类型进行转换
                if (underlyingType == typeof(string))
                {
                    convertedValue = value.ToString();
                }
                else if (underlyingType == typeof(int) && int.TryParse(value, out var intValue))
                {
                    convertedValue = intValue;
                }
                else if (
                    underlyingType == typeof(DateTime)
                    && DateTime.TryParse(value, out var dateTimeValue)
                )
                {
                    convertedValue = dateTimeValue;
                }
                else if (underlyingType == typeof(bool) && bool.TryParse(value, out var boolValue))
                {
                    convertedValue = boolValue;
                }
                else if (
                    underlyingType == typeof(double)
                    && double.TryParse(value, out var doubleValue)
                )
                {
                    convertedValue = doubleValue;
                }
                else if (
                    underlyingType == typeof(decimal)
                    && decimal.TryParse(value, out var decimalValue)
                )
                {
                    convertedValue = decimalValue;
                }

                // 如果成功转换了值，则设置属性
                if (convertedValue != null)
                {
                    // 使用缓存的委托设置属性值
                    var setterKey = $"{type.FullName}.{propertyName}";
                    var setter = PropertySetterCache.GetOrAdd(
                        setterKey,
                        _ => CreateSetter<T>(property)
                    );

                    if (setter is Action<T, object> typedSetter)
                    {
                        typedSetter(result, convertedValue);
                    }
                }
            }
            catch
            {
                // 忽略转换异常，继续处理下一个属性
            }
        }

        return result;
    }

    /// <summary>
    /// 创建属性设置器委托
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="property">属性信息</param>
    /// <returns>设置属性值的委托</returns>
    private static Delegate CreateSetter<T>(PropertyInfo property)
    {
        // 创建表达式树参数: (T instance, object value)
        var instanceParam = Expression.Parameter(typeof(T), "instance");
        var valueParam = Expression.Parameter(typeof(object), "value");

        // 创建属性访问表达式: instance.Property
        var propertyAccess = Expression.Property(instanceParam, property);

        // 创建值转换表达式: (PropertyType)value
        var convertedValue = Expression.Convert(valueParam, property.PropertyType);

        // 创建赋值表达式: instance.Property = (PropertyType)value
        var assignExpression = Expression.Assign(propertyAccess, convertedValue);

        // 创建 lambda 表达式: (T instance, object value) => instance.Property = (PropertyType)value
        var lambda = Expression.Lambda<Action<T, object>>(
            assignExpression,
            instanceParam,
            valueParam
        );

        // 编译表达式树为委托
        return lambda.Compile();
    }
}
