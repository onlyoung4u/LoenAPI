using LoenAPI.Common;
using Microsoft.AspNetCore.Http;

namespace LoenAPI.Extensions;

/// <summary>
/// 响应扩展
/// </summary>
public static class LoenResponseExtensions
{
    /// <summary>
    /// 成功
    /// </summary>
    /// <returns></returns>
    public static IResult Success()
    {
        return Results.Ok(new ApiResponse());
    }

    /// <summary>
    /// 成功
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public static IResult Success<T>(T data)
    {
        return Results.Ok(new ApiResponse<T>(ResponseCode.Success, null, data));
    }

    /// <summary>
    /// 错误
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static IResult Error(string? message = null)
    {
        return Results.Ok(new ApiResponse(ResponseCode.Error, message));
    }

    /// <summary>
    /// 错误
    /// </summary>
    /// <param name="message"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public static IResult Error(string message, ResponseCode code)
    {
        return Results.Ok(new ApiResponse(code, message));
    }

    /// <summary>
    /// 成功或错误
    /// </summary>
    /// <param name="success"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static IResult SuccessOrError(bool success, string? message = null)
    {
        return success ? Success() : Error(message);
    }

    /// 错误
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static IResult ErrorWithCode(ResponseCode code)
    {
        return Results.Ok(new ApiResponse(code));
    }

    /// <summary>
    /// 参数错误
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static IResult ErrorParams(string? message = null)
    {
        return Results.Ok(new ApiResponse(ResponseCode.InvalidParams, message));
    }

    /// <summary>
    /// 未授权
    /// </summary>
    /// <returns></returns>
    public static IResult Unauthorized()
    {
        return Results.Ok(new ApiResponse(ResponseCode.Unauthorized));
    }

    /// <summary>
    /// 禁止访问
    /// </summary>
    /// <returns></returns>
    public static IResult Forbidden()
    {
        return Results.Ok(new ApiResponse(ResponseCode.Forbidden));
    }
}
