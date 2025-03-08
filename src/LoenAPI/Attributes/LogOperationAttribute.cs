namespace LoenAPI.Attributes;

/// <summary>
/// 标记需要记录操作日志的端点
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class LogOperationAttribute : Attribute
{
    /// <summary>
    /// 是否记录请求体
    /// </summary>
    public bool LogRequestBody { get; set; } = true;

    /// <summary>
    /// 是否记录响应体
    /// </summary>
    public bool LogResponseBody { get; set; } = false;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public LogOperationAttribute() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="description">操作描述</param>
    public LogOperationAttribute(string description)
    {
        Description = description;
    }
}
