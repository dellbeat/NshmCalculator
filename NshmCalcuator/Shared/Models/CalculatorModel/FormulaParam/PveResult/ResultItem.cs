using System.Text.Json.Serialization;

namespace NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Result;

/// <summary>
/// 表示结果上的一个点
/// </summary>
public class ResultItem
{
    /// <summary>
    /// 需要引用的结果公式代号
    /// </summary>
    public string Code { get; set; } 
    
    /// <summary>
    /// 引用的结果值
    /// </summary>
    public double Value { get; set; }
    
    /// <summary>
    /// 标题名称
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    /// 包含特殊名称的Json
    /// </summary>
    public PointTitle? SpecialPointTitle { get; set; }
    
    /// <summary>
    /// 备注（待定）
    /// </summary>
    public string Remark { get; set; }
}