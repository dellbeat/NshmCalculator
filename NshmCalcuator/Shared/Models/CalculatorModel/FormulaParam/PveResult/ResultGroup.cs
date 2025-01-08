using System.Text.Json.Serialization;
using NshmCalculator.Shared.Models.CalculatorModel.Enums;

namespace NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Result;

public class ResultGroup
{
    /// <summary>
    /// 组名称
    /// </summary>
    public string GroupName { get; set; }
    
    /// <summary>
    /// 所有线的集合
    /// </summary>
    public ResultLine[] ResultLines { get; set; }
    
    /// <summary>
    /// 结果需要的展示模式
    /// </summary>
    public ResultGroupEnum GroupMode { get; set; }
    
    /// <summary>
    /// 引用的参数列表
    /// </summary>
    public List<string> RefCodeList { get; set; }
    
    /// <summary>
    /// 图表内的小数位数，目前仅控制图表
    /// </summary>
    public int DecimalPlace { get; set; }
    
    /// <summary>
    /// 用于传递给组件的标识GUID
    /// </summary>
    [JsonIgnore]
    public string RenderGuid { get; set; }
}