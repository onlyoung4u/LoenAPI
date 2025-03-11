namespace LoenAPI.Dtos;

/// <summary>
/// 分页参数
/// </summary>
public class PaginationRequest
{
    /// <summary>
    /// 页码
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// 每页条数
    /// </summary>
    public int Limit { get; set; } = 10;
}

/// <summary>
/// 分页响应
/// </summary>
/// <typeparam name="T"></typeparam>
public class PaginationResponse<T>
{
    /// <summary>
    /// 列表
    /// </summary>
    public List<T> List { get; set; }

    /// <summary>
    /// 总条数
    /// </summary>
    public int Total { get; set; }
}
