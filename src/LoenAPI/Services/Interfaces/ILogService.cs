using LoenAPI.Dtos;
using LoenAPI.Models;

namespace LoenAPI.Services.Interfaces;

public interface ILogService
{
    /// <summary>
    /// 获取操作日志
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<PaginationResponse<LoenOperationLog>> GetLogs(PaginationRequest request);
}
