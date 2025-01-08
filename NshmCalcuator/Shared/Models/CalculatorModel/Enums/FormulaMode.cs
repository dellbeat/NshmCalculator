namespace NshmCalculator.Shared.Models.CalculatorModel.Enums;

/// <summary>
/// 公式中的特殊处理规则枚举
/// </summary>
public enum FormulaMode
{
    /// <summary>
    /// 默认值，在规则检查中视为非法值
    /// </summary>
    Invalid,
    /// <summary>
    /// 需要调用时传入外部数值方可使用的公式
    /// </summary>
    Lambda,
    /// <summary>
    /// 内功，需要进行关联
    /// </summary>
    Ki,
    /// <summary>
    /// 需要调用公式
    /// </summary>
    LinkLambda
}