using NshmCalculator.Shared.Models.CalculatorModel;
using NshmCalculator.Shared.Models.CalculatorModel.CalculatorConfig;

namespace NshmCalculator.Shared.Models.PageModel;

public class PvePageModel
{
	/// <summary>
	/// 已加载的数据配置
	/// </summary>
	public PveConfig CurrentConfig { get; set; }
}