// ReSharper disable ClassNeverInstantiated.Global

using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Params;
using NshmCalculator.Shared.Models.Interface;

namespace NshmCalculator.Shared.Models.CalculatorModel.CalculatorConfig;

/// <summary>
/// 升级内功词条计算器的界面配置
/// </summary>
public class KiUpdateConfig : IGameConfig
{
    /// <summary>
    /// 原文件版本号，对应界面上的引用
    /// </summary>
    public string Version { get; set; }

    public long InternalVersion { get; set; }

    /// <summary>
    /// 参与计算的属性词条默认值列表
    /// </summary>
    public KiUpdateAttribute[] AttributeArray { get; set; } = [];

    public string HelperText { get; set; }
}
