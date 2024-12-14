using NshmCalculator.Shared.Models.CalculatorModel;
using NshmCalculator.Shared.Models.CalculatorModel.CalculatorConfig;

namespace NshmCalculator.Shared.Models.PageModel;

public class PvePageModel
{
    /// <summary>
    /// 根据选项代号记录其当前值，供计算类使用
    /// </summary>
    public Dictionary<string,ParamValue> ParamValuesDictionary { get; set; }
}