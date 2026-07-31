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
        TreatIntensity = 1500,
        CriticalHits = 1534,
        ExtraCriticalHitsRate = 0,
        CriticalDamageRate = 150,
        CureGain = 170
    };

    /// <summary>
    /// 疗承比（内功总收益的全局归一化旋钮，默认 1.3）
    /// </summary>
    public double TreatDefenseRatio { get; set; } = 1.3;

    /// <summary>
    /// 灵韵下拉框选中项名称（"无"表示不选；选中灵韵内功后其收益值会动态计算并计入总收益）
    /// <para>4.1.1 内功收益改造后，灵韵下拉框简化为「无/有」，UI 角色由 <see cref="HasLingYun"/> 接管；
    /// 此字段保留用于 OCR 识别逻辑向后兼容（OCR 暂未对接新下拉，后续改动迁移）。</para>
    /// </summary>
    public string LingYunSelection { get; set; } = "无";

    /// <summary>
    /// 内功名称下拉框选中项（"无"表示不选；选中特效内功后其特性收益 A 会动态查表并计入总收益）
    /// <para>选项来源：配置「内功特性收益」组（排除御千山「无敌」文本、绝电鸿音未配置项）。</para>
    /// </summary>
    public string NeiGongSelection { get; set; } = "无";

    /// <summary>
    /// 灵韵下拉框选中项（"无"/"有"）。选「有」时按 <see cref="NeiGongSelection"/> 查「灵韵收益」表计入 B。
    /// </summary>
    public string HasLingYun { get; set; } = "无";

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
    /// 内功特性收益 A（由「内功名称」下拉选中项查 RF_TS_* 求得，显示用，不计入持久化）
    /// </summary>
    public double TeSeGain { get; set; }

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
