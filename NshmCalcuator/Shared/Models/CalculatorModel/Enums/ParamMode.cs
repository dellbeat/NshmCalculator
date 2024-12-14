namespace NshmCalculator.Shared.Models.CalculatorModel.Enums;

/// <summary>
/// 选项应填的内容
/// </summary>
public enum ParamMode
{
    /// <summary>
    /// 普通数字
    /// </summary>
    Number,
    /// <summary>
    /// 百分比
    /// </summary>
    Percent,
    /// <summary>
    /// 下拉菜单
    /// </summary>
    Selection,
    /// <summary>
    /// 动态文本
    /// </summary>
    DynamicText
}