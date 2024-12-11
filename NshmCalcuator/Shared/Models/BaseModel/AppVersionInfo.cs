namespace NshmCalculator.Shared.Models.BaseModel;

/// <summary>
/// 记录应用内所有在更的计算器内部版本号，供页面判断是否需要从网络请求
/// </summary>
public class AppVersionInfo
{
    /// <summary>
    /// 计算器的内部版本号字典
    /// </summary>
    public Dictionary<string, long> ConfigVersionInfo { get; set; }
    
    /// <summary>
    /// 配置所在的路径
    /// </summary>
    public Dictionary<string,string> ConfigPathInfo { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public string UpdateTime { get; set; }
}