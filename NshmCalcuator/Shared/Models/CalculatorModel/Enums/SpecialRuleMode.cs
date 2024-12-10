namespace NshmCalculator.Shared.Models.CalculatorModel.Enums;

public enum SpecialRuleMode
{
	/// <summary>
	/// 通过指定前置代号并比较值加载对应的文本
	/// </summary>
	CompareOptions,
    /// <summary>
    /// 删除选项中与特定参数已选内容一致的选项
    /// </summary>
    RemoveSameOptions
}