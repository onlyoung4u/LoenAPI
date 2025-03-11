using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using LoenAPI.Authentication;
using LoenAPI.Caching;
using LoenAPI.Services;
using LoenAPI.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using StackExchange.Redis;

namespace LoenAPI.Extensions;

/// <summary>
/// 服务集合扩展
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加Loen服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddLoenServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var loenAdmin = configuration.GetSection("Loen");

        var isDevelopment = loenAdmin.GetValue<string>("Env") == "dev";
        var databaseType =
            loenAdmin.GetValue<string>("DatabaseType")?.ToLower() == "mysql"
                ? DbType.MySql
                : DbType.PostgreSQL;
        var databaseConnection =
            loenAdmin.GetValue<string>("Database") ?? "DefaultDatabaseConnection";

        var databaseConnectionString = configuration.GetConnectionString(databaseConnection);
        var redisConnectionString =
            configuration.GetConnectionString("RedisConnection") ?? "localhost:6379";

        if (string.IsNullOrEmpty(databaseConnectionString))
        {
            throw new Exception("DatabaseConnection is not configured");
        }

        // 配置Json序列化选项
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
            // options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        // 基础设施服务
        services.AddSingleton<ISqlSugarClient>(s =>
            CreateSqlSugarClient(databaseType, databaseConnectionString, isDevelopment)
        );

        // 添加跨域服务
        services.AddLoenCors(configuration);

        // 缓存服务
        services.AddMemoryCache();
        services.AddRedisCache(redisConnectionString);
        services.AddCacheProviders();

        // JWT服务
        services.Configure<List<JwtOptions>>(configuration.GetSection("Jwt"));
        services.AddSingleton<IJwtService, JwtService>();

        // 权限服务
        services.AddSingleton<PermissionService>();

        // 验证服务
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped<IValidationService, ValidationService>();

        // 业务相关服务
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<ILogService, LogService>();

        return services;
    }

    private static SqlSugarScope CreateSqlSugarClient(
        DbType dbType,
        string connectionString,
        bool isDevelopment
    )
    {
        return new SqlSugarScope(
            new ConnectionConfig()
            {
                DbType = dbType,
                ConnectionString = connectionString,
                IsAutoCloseConnection = true,
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    EntityService = (x, p) =>
                        p.DbColumnName = UtilMethods.ToUnderLine(p.DbColumnName),
                    EntityNameService = (x, p) =>
                        p.DbTableName = UtilMethods.ToUnderLine(p.DbTableName),
                },
            },
            db =>
            {
                if (isDevelopment)
                {
                    db.Aop.OnLogExecuting = (sql, pars) => Console.WriteLine(sql);
                }
            }
        );
    }

    private static IServiceCollection AddRedisCache(
        this IServiceCollection services,
        string connectionString
    )
    {
        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(connectionString)
        );

        return services;
    }

    private static IServiceCollection AddCacheProviders(this IServiceCollection services)
    {
        services.AddSingleton<MemoryCacheProvider>();
        services.AddSingleton<RedisCacheProvider>();
        services.AddSingleton<CacheService>();

        return services;
    }

    /// <summary>
    /// 添加Loen跨域服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    private static IServiceCollection AddLoenCors(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var corsOptions = configuration.GetSection("Cors");
        var origins = corsOptions.GetSection("AllowedOrigins").Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy(
                "LoenCorsPolicy",
                policy =>
                {
                    if (origins.Length > 0)
                    {
                        policy
                            .WithOrigins(origins)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    }
                    else
                    {
                        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                    }
                }
            );
        });

        return services;
    }
}
