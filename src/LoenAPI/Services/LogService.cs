using LoenAPI.Dtos;
using LoenAPI.Models;
using LoenAPI.Services.Interfaces;
using SqlSugar;

namespace LoenAPI.Services;

public class LogService(ISqlSugarClient sqlSugarClient) : ILogService
{
    private readonly ISqlSugarClient _sqlSugarClient = sqlSugarClient;

    public async Task<PaginationResponse<LoenOperationLog>> GetLogs(
        PaginationRequest request,
        LogRequest logRequest
    )
    {
        RefAsync<int> total = 0;

        var logs = await _sqlSugarClient
            .Queryable<LoenOperationLog>()
            .WhereIF(
                logRequest.UserId.HasValue && logRequest.UserId > 0,
                x => x.UserId == logRequest.UserId
            )
            .WhereIF(logRequest.StartTime.HasValue, x => x.CreatedAt >= logRequest.StartTime)
            .WhereIF(logRequest.EndTime.HasValue, x => x.CreatedAt <= logRequest.EndTime)
            .OrderByDescending(x => x.Id)
            .ToOffsetPageAsync(request.Page, request.Limit, total);

        return new PaginationResponse<LoenOperationLog> { List = logs, Total = total.Value };
    }
}
