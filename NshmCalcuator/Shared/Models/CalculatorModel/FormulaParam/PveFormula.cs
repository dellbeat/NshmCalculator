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
}