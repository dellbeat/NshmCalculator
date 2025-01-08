using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.SpecialRule;

namespace NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Formula;

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
    /// 所属分组代码，供计算结果时分组使用
    /// </summary>
    public string? BelongResultCode { get; set; }

    /// <summary>
    /// 计算公式
    /// </summary>
    public string Formula { get; set; }

    /// <summary>
    /// 计算公式参数列表
    /// </summary>
    public string[] FormulaParam { get; set; }
    
    /// <summary>
    /// 公式特殊处理规则
    /// </summary>
    public List<SpecialFormulaRule> Rule { get; set; }
    
    /// <summary>
    /// 标记公式是否完全可用
    /// </summary>
    public bool NotComplete { get; set; }
    
    /// <summary>
    /// 标记公式层次，为0系公式不完全可用所致
    /// </summary>
    public int Level { get; set; }
}