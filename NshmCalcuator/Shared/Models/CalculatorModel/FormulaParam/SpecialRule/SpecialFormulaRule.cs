using NshmCalculator.Shared.Models.CalculatorModel.Enums;

namespace NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.SpecialRule;

/// <summary>
/// 特殊的公式规则
/// </summary>
public class SpecialFormulaRule
{
    /// <summary>
    /// 规则类型
    /// </summary>
    public FormulaMode Mode { get; set; }
    
    /// <summary>
    /// 当为<see cref="FormulaMode.Lambda">需传参公式</see>时包含的参数或为<see cref="FormulaMode.LinkLambda">调用公式</see>时包含的参数
    /// </summary>
    public string[] LambdaParam { get; set; }
}