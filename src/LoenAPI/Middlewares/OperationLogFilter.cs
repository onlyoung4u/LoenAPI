using System.Text;
using LoenAPI.Common;
using LoenAPI.Extensions;
using LoenAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;
using SqlSugar;

namespace LoenAPI.Middlewares;

public class OperationLogFilter : IEndpointFilter
{
    private readonly ISqlSugarClient _db;

    public OperationLogFilter(ISqlSugarClient db)
    {
        _db = db;
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        var endpoint = context.HttpContext.GetEndpoint();
        if (endpoint == null)
        {
            return await next(context);
        }

        // 获取路由名称
        var routeName = endpoint.Metadata.GetMetadata<EndpointNameMetadata>()?.EndpointName;
        if (string.IsNullOrEmpty(routeName))
        {
            return await next(context);
        }

        // 过滤 GET 请求
        if (context.HttpContext.Request.Method == HttpMethod.Get.Method)
        {
            return await next(context);
        }

        // 获取用户信息
        var userId = context.HttpContext.GetUserId();
        var user = await _db.Queryable<LoenUser>().Where(x => x.Id == userId).FirstAsync();

        // 获取请求体
        string requestBody = string.Empty;
        if (context.HttpContext.Request.ContentLength > 0)
        {
            context.HttpContext.Request.EnableBuffering();
            using var reader = new StreamReader(
                context.HttpContext.Request.Body,
                Encoding.UTF8,
                leaveOpen: true
            );
            requestBody = await reader.ReadToEndAsync();
            context.HttpContext.Request.Body.Position = 0;
        }

        var log = new LoenOperationLog
        {
            UserId = userId,
            Username = user?.Username ?? string.Empty,
            Nickname = user?.Nickname ?? string.Empty,
            Path = context.HttpContext.Request.GetDisplayUrl(),
            Route = routeName,
            Method = context.HttpContext.Request.Method,
            Ip = context.HttpContext.GetRealIp(),
            Body = requestBody,
        };

        object? result = null;

        try
        {
            // 执行下一个过滤器
            result = await next(context);

            // 判断是否成功
            if (context.HttpContext.Response.StatusCode == StatusCodes.Status200OK)
            {
                if (result is ApiResponse response)
                {
                    log.Success = response.Code == (int)ResponseCode.Success;
                }
            }
        }
        catch
        {
            log.Success = false;
            throw; // 重新抛出异常
        }
        finally
        {
            // 确保无论如何都会保存日志
            try
            {
                await _db.Insertable(log).ExecuteCommandAsync();
            }
            catch
            {
                // 这里可以选择记录日志保存失败的错误，但不影响主流程
            }
        }

        return result;
    }
}
