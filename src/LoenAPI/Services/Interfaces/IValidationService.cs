using FluentValidation;

namespace LoenAPI.Services.Interfaces;

/// <summary>
/// 验证服务接口
/// </summary>
public interface IValidationService
{
    /// <summary>
    /// 验证请求对象
    /// </summary>
    /// <typeparam name="T">请求对象类型</typeparam>
    /// <param name="request">请求对象</param>
    /// <param name="validator">验证器</param>
    /// <returns>验证后的请求对象</returns>
    Task<T> ValidateAsync<T>(T? request, IValidator<T> validator);
}
