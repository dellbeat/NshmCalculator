namespace NshmCalculator.Shared.Models.CalculatorModel;

/// <summary>
/// 界面动态参数配置
/// </summary>
public class FrontParamInfo
{
	/// <summary>
	/// 参数唯一标识符
	/// </summary>
	public string Code { get; set; }
	
	/// <summary>
	/// 标签名称
	/// </summary>
	public string Name { get; set; }
	
	/// <summary>
	/// 是否为下拉框模式，如此项选中百分比模式无效
	/// </summary>
	public bool ComboBoxMode { get; set; }
	
	/// <summary>
	/// 是否为百分比模式，如为百分比模式，文本框最后会带百分号
	/// </summary>
	public bool PercentageMode { get; set; }
	
	/// <summary>
	/// 下拉框模式时的选项列表
	/// </summary>
	public string?[] Options { get; set; }
	
	/// <summary>
	/// 分组名，如为NULL则不作分组处理
	/// </summary>
	public string? GroupName { get; set; }
}