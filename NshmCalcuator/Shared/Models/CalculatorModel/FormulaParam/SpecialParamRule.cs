using NshmCalculator.Shared.Models.CalculatorModel.Enums;

namespace NshmCalculator.Shared.Models.CalculatorModel;

public class SpecialParamRule
{
    /// <summary>
    /// 前置的参数代号
    /// </summary>
    public string FrontParamCode { get; set; }

    /// <summary>
    /// 规则类型
    /// </summary>
    public SpecialRuleMode Mode { get; set; }

    /// <summary>
    /// 在<see cref="SpecialRuleMode">对照加载文本</see>模式下需要关联的键值对
    /// </summary>
    public Dictionary<string, string> TextDic { get; set; }
}