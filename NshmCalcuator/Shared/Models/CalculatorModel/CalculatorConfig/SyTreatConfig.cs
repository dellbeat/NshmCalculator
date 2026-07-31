using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Formula;
using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Params;
using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Result;
using NshmCalculator.Shared.Models.Interface;

namespace NshmCalculator.Shared.Models.CalculatorModel.CalculatorConfig;

/// <summary>
/// 师尹的素鸿治疗计算器页面的数据配置（4.1.1 起 NCalc 配置驱动，对齐 PVE 架构）
/// </summary>
public class SyTreatConfig : IGameConfig
{
    /// <summary>
    /// 原文件版本，对应界面上的引用
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// 组别列表，确定界面渲染顺序
    /// </summary>
    public string[] CategoryArray { get; set; }

    /// <summary>
    /// 前端参数列表（包含基础面板、属性变化、转换疗强、PVP 词条收益等所有输入项）
    /// </summary>
    public FrontParamInfo[] FrontParamInfoArray { get; set; }

    /// <summary>
    /// 中间公式（系数表常量、会心率、奶量、K23 分组小计等）
    /// </summary>
    public PveFormula[] InternalFormulas { get; set; }

    /// <summary>
    /// 结果公式（奶量、K23 总收益、评级、灵韵收益参考、周天收益等）
    /// </summary>
    public PveFormula[] ResultFormulas { get; set; }

    /// <summary>
    /// 结果展示分组（对应界面的结果 Tab）
    /// </summary>
    public ResultGroup[] ResultGroups { get; set; }

    /// <summary>
    /// 参数默认值（key 与 FrontParamInfoArray 的 Code 对应）
    /// </summary>
    public Dictionary<string, ParamValue> DefaultParamValues { get; set; }

    public long InternalVersion { get; set; }

    public string HelperText { get; set; }
}
