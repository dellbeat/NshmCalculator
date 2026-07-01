// ReSharper disable ClassNeverInstantiated.Global

namespace NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Params;

/// <summary>
/// 升级内功词条计算器中的单条属性参数
/// </summary>
public class KiUpdateAttribute
{
    /// <summary>
    /// 属性名称，如 攻击/会心/破防 等，由配置固定，界面上只读
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 数值上限（一条词条的最大数值，默认值可由配置/用户调整）
    /// </summary>
    public double MaxValue { get; set; }

    /// <summary>
    /// 数值下限（一条词条的最小数值，默认值可由配置/用户调整）
    /// </summary>
    public double MinValue { get; set; }

    /// <summary>
    /// 上限收益率（满额时的收益率，默认值可由配置/用户调整）
    /// </summary>
    public double MaxRate { get; set; }

    /// <summary>
    /// 填写数值（用户输入的实际词条数值，未填写为 null）
    /// </summary>
    public double? InputValue { get; set; }
}
