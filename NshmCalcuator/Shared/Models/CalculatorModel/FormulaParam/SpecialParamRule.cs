using NshmCalculator.Shared.Models.CalculatorModel.Enums;

namespace NshmCalculator.Shared.Models.CalculatorModel;

public class SpecialParamRule
{
    /// <summary>
    /// 前置/关联(在<see cref="ParamRuleMode.ShareOptions"/>>)的参数代号
    /// </summary>
    public string FrontParamCode { get; set; }
    
    /// <summary>
    /// 在<see cref="ParamRuleMode.CompareOptions">比对选项模式</see>下需要赋值的参数代号
    /// </summary>
    public string RelatedParamCode { get; set; }

    /// <summary>
    /// 规则类型
    /// </summary>
    public ParamRuleMode Mode { get; set; }

    /// <summary>
    /// 自定义键值对
    /// </summary>
    public Dictionary<string, object> TextDic { get; set; }
}