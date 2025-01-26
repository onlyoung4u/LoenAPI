using FluentValidation;
using LoenAPI.Exceptions;
using LoenAPI.Services.Interfaces;

namespace LoenAPI.Services;

/// <summary>
/// 验证服务
/// </summary>
public class ValidationService : IValidationService
{
    /// <summary>
    /// 验证请求对象
    /// </summary>
    public async Task<T> ValidateAsync<T>(T? request, IValidator<T> validator)
    {
        if (request == null)
        {
            throw new LoenValidationException();
        }

        var result = await validator.ValidateAsync(request);

        if (!result.IsValid)
        {
            throw new LoenValidationException(result.Errors.FirstOrDefault()?.ErrorMessage);
        }

        return request;
    }
}
