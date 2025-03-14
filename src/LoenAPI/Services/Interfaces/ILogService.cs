using LoenAPI.Dtos;
using LoenAPI.Models;

namespace LoenAPI.Services.Interfaces;

public interface ILogService
{
    /// <summary>
    /// 获取操作日志
    /// </summary>
    /// <param name="request">分页请求</param>
    /// <param name="logRequest">日志查询条件</param>
    /// <returns>分页响应</returns>
    Task<PaginationResponse<LoenOperationLog>> GetLogs(
        PaginationRequest request,
        LogRequest logRequest
    );
}
