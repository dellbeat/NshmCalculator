using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Params;

namespace NshmCalculator.Shared.Models.PageModel;

/// <summary>
/// 升级内功词条计算器的页面数据模型，用于本地存储缓存与恢复
/// </summary>
public class KiUpdatePageModel
{
    /// <summary>
    /// 界面上存储的属性词条数据及填写数值
    /// </summary>
    public KiUpdateAttribute[] AttributeArray { get; set; } = [];

    /// <summary>
    /// 当前加载的内部版本
    /// </summary>
    public long CurrentInternalVersion { get; set; } = -99999;
}
