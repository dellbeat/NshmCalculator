namespace NshmCalculator.Shared.Models.CalculatorModel.Enums;

public enum ParamRuleMode
{
    /// <summary>
    /// 默认值，在规则检查中视为非法值
    /// </summary>
    Invalid,
	/// <summary>
	/// 通过指定前置代号并比较值加载对应的文本
	/// </summary>
	CompareOptions,
    /// <summary>
    /// 删除选项中与特定参数已选内容一致的选项
    /// </summary>
    RemoveSameOptions,
    /// <summary>
    /// 共享选项
    /// </summary>
    ShareOptions,
    /// <summary>
    /// 选择时自动给关联选项赋值，用于选择BOSS时更新相关数值
    /// </summary>
    AssignEnemyData,
    /// <summary>
    /// 根据前置代号的值控制自身是否可见
    /// </summary>
    ControlRender,
    /// <summary>
    /// 检查特定选项值，并根据关联选项的值赋值给自己（用于动态文本）
    /// </summary>
    RelatedAssignment
}