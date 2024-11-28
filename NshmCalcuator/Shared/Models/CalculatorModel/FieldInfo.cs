using System.Text.Json.Serialization;

namespace NshmCalculator.Shared.Models.CalculatorModel;

/// <summary>
/// 素问治疗计算器动态系数配置
/// </summary>
public class FieldInfo
{
    /// <summary>
    /// 展示在界面上的名称
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// 乘数系数列表
    /// </summary>
    public List<double> MultiCoeList { get; set; }
    
    /// <summary>
    /// 除数系数列表
    /// </summary>
    public List<double> ExceptCoeList { get; set; }
    
    /// <summary>
    /// 除以疗坦率列表
    /// </summary>
    public List<bool> ExceptRatioList { get; set; }
    
    /// <summary>
    /// 所属组别，在需要分组时设置
    /// </summary>
    public string? BelongGroup { get; set; }
    
    /// <summary>
    /// 百分比模式
    /// </summary>
    public bool PercentMode { get; set; }
    
    /// <summary>
    /// 界面值
    /// </summary>
    [JsonIgnore]
    public int Value { get; set; }
    
    /// <summary>
    /// 属性数值在EXCEL上对应的地址，仅供自动处理使用
    /// </summary>
    public string CellAddress { get; set; }
}