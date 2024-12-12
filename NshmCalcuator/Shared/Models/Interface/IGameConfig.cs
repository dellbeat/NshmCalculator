namespace NshmCalculator.Shared.Models.Interface;

/// <summary>
/// 计算器配置项接口
/// </summary>
public interface IGameConfig
{
    /// <summary>
    /// 配置内部版本号，以日期+时间+次序进行标识
    /// </summary>
    long InternalVersion { get; set; }
    
    /// <summary>
    /// 显示在弹窗内的帮助文本
    /// </summary>
    string HelperText { get; set; }
}