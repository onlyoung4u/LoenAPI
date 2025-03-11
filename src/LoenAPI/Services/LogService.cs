using LoenAPI.Dtos;
using LoenAPI.Models;
using LoenAPI.Services.Interfaces;
using SqlSugar;

namespace LoenAPI.Services;

public class LogService(ISqlSugarClient sqlSugarClient) : ILogService
{
    private readonly ISqlSugarClient _sqlSugarClient = sqlSugarClient;

    public async Task<PaginationResponse<LoenOperationLog>> GetLogs(PaginationRequest request)
    {
        RefAsync<int> total = 0;

        var logs = await _sqlSugarClient
            .Queryable<LoenOperationLog>()
            .OrderByDescending(x => x.Id)
            .ToOffsetPageAsync(request.Page, request.Limit, total);

        return new PaginationResponse<LoenOperationLog> { List = logs, Total = total.Value };
    }
}
