using NshmCalculator.Shared.Models.CalculatorModel;
using NshmCalculator.Shared.Models.CalculatorModel.CalculatorConfig;
using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Params;

namespace NshmCalculator.Shared.Models.PageModel;

public class PvePageModel
{
    /// <summary>
    /// 根据选项代号记录其当前值，供计算类使用
    /// </summary>
    public Dictionary<string,ParamValue> ParamValuesDictionary { get; set; } = null;
    
    /// <summary>
    /// 计算时/升级时记录当前配置版本，用于给可能的升级迁移策略提供参考
    /// </summary>
    public long CurrentVersion { get; set; }
}