using System.ComponentModel.DataAnnotations;
using NshmCalculator.Shared.Models.CalculatorModel;
using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Params;

namespace NshmCalculator.Shared.Models.PageModel;

/// <summary>
/// 素鸿治疗计算器页面状态模型（4.1.1 起 NCalc 配置驱动，对齐 PvePageModel 结构）
/// <para>不再使用 [JsonUnmappedMemberHandling.Disallow]：schema 变化大，改为宽松模式 + InternalVersion 触发迁移</para>
/// </summary>
public class SyTreatPageModel
{
    /// <summary>
    /// 配置驱动的参数值字典（key=FrontParamInfo.Code，对应转换疗强/PVP 词条等配置驱动 Tab）
    /// </summary>
    public Dictionary<string, ParamValue> ParamValuesDictionary { get; set; } = new();

    /// <summary>
    /// 基础治疗面板（奶量对比 Tab 的 EditForm 用，强类型便于校验）
    /// </summary>
    [ValidateComplexType]
    public TreatInfo BaseTreatInfo { get; set; } = new TreatInfo()
    {
        TreatIntensity = 6290,
        CriticalHits = 1500,
        ExtraCriticalHitsRate = 3,
        CriticalDamageRate = 160,
        CureGain = 170
    };

    /// <summary>
    /// 疗承比（内功总收益的全局归一化旋钮，默认 1.3）
    /// </summary>
    public double TreatDefenseRatio { get; set; } = 1.3;

    /// <summary>
    /// 灵韵下拉框选中项名称（"无"表示不选；选中灵韵内功后其收益值会动态计算并计入总收益）
    /// </summary>
    public string LingYunSelection { get; set; } = "无";

    /// <summary>
    /// 当前 Tab 索引（0=属性变化, 1=PVP词条），用于控制计算分发与 UI
    /// </summary>
    public int ActiveIndex { get; set; }

    #region 属性变化计算面板（Tab0 的 Δ 输入）

    /// <summary>
    /// Δ治疗强度
    /// </summary>
    public int TreatIntensity { get; set; }

    /// <summary>
    /// Δ会心
    /// </summary>
    public int CriticalHits { get; set; }

    /// <summary>
    /// Δ会伤百分比
    /// </summary>
    public double CriticalDamageRate { get; set; }

    /// <summary>
    /// Δ额外会心率百分比
    /// </summary>
    public double ExtraCriticalHitsRate { get; set; }

    /// <summary>
    /// Δ疗效增益百分比（与 BaseTreatInfo.CureGain 相加参与奶量公式）
    /// </summary>
    public double CureGain { get; set; }

    /// <summary>
    /// 变化后计算会心率（结果显示）
    /// </summary>
    public double CalculateCriticalHitsRate { get; set; }

    /// <summary>
    /// 变化后治疗量（结果显示）
    /// </summary>
    public double CalculateTreatNum { get; set; }

    /// <summary>
    /// 治疗量增值（变化后 - 基础）
    /// </summary>
    public double TreatGrowthNum { get; set; }

    /// <summary>
    /// 治疗量增值百分比
    /// </summary>
    public double TreatGrowthPercent { get; set; }

    #endregion

    #region 结果输出

    /// <summary>
    /// PVP 总评分
    /// </summary>
    public double Score { get; set; }

    /// <summary>
    /// 评级文本（S+/S/A/B/C）
    /// </summary>
    public string GradeText { get; set; }

    #endregion

    /// <summary>
    /// 配置的内部版本号（用于触发缓存迁移）
    /// </summary>
    public long InternalVersion { get; set; }
}
