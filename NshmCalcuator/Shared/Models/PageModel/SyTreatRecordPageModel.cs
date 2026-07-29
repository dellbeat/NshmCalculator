using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Params;

namespace NshmCalculator.Shared.Models.PageModel;

/// <summary>
/// 素问治疗计算器中保存的内功记录列表的页面数据模型，用于本地存储缓存与恢复
/// </summary>
public class SyTreatRecordPageModel
{
    /// <summary>
    /// 已保存的内功记录列表
    /// </summary>
    public List<SyTreatRecord> Records { get; set; } = new();
}
