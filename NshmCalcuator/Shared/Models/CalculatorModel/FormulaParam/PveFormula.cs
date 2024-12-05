namespace NshmCalculator.Shared.Models.CalculatorModel;

/// <summary>
/// 新PVE计算器公式类
/// </summary>
public class PveFormula
{
    /// <summary>
    /// 唯一标识代码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 计算公式
    /// </summary>
    public string Formula { get; set; }

    /// <summary>
    /// 计算公式参数列表
    /// </summary>
    public string[] FormulaParam { get; set; }
    
    /// <summary>
    /// 如为传参公式涉及到的参数
    /// </summary>
    public string?[] LamParam{get;set;}
    
	/// <summary>
	/// 是否为需要传参的公式
	/// </summary>
    public bool LamMode{get;set;}
    
    /// <summary>
    /// 标记公式是否完全可用
    /// </summary>
    public bool NotComplete { get; set; }
    
    /// <summary>
    /// 标记公式层次，为0系公式不完全可用所致
    /// </summary>
    public int Level { get; set; }
}