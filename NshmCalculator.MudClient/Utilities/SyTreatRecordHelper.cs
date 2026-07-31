using NshmCalculator.Shared;
using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Params;

namespace NshmCalculator.MudClient.Utilities;

/// <summary>
/// 素鸿治疗计算器内功记录复算工具。
/// 严格复刻 <c>SyTreatCalculator.PvpCalculate</c> 的核心流程（装字典 → 算特性 A → 算灵韵 B → 算 RF_TotalGain/RF_Grade），
/// 供页面与编辑弹窗共用，保证记录总收益与页面计算结果完全一致。
/// <para>4.1.1 内功收益改造后：内功名称下拉控制特性收益 A(ST_PVP_TeSe)，灵韵下拉简化为「无/有」控制灵韵收益 B(ST_PVP_LingYun)，
/// 二者按内功名称关联查表，可叠加计入总收益。</para>
/// <para>引擎 <see cref="TreatUtility"/> 为静态类，由页面 <c>InitParameters</c> 初始化一次，
/// 弹窗与页面共享同一引擎实例，无需在此重新初始化。</para>
/// </summary>
public static class SyTreatRecordHelper
{
    /// <summary>结果公式代号（与页面 <c>_pvpCodes</c> 保持一致）</summary>
    private static readonly List<string> PvpCodes = new() { "RF_TotalGain", "RF_Grade" };

    /// <summary>
    /// 基于记录的词条数值 + 内功名称/灵韵选择 + 页面疗承比，调用引擎复算 (总收益, 评级)。
    /// 流程严格复刻 <c>SyTreatCalculator.PvpCalculate</c>：装字典 → 算特性 A → 算灵韵 B → 算总收益/评级。
    /// </summary>
    /// <param name="record">被复算的内功记录（读取 InputValues/NeiGongSelection/HasLingYun；旧记录回退 LingYunSelection）</param>
    /// <param name="pageTankRatio">页面当前疗承比（疗承比跟随页面上下文，不存入记录）</param>
    /// <param name="lingyunNameToCode">灵韵名称 -> RF_LY_* Code 映射</param>
    /// <param name="neigongNameToCode">内功名称 -> RF_TS_* Code 映射（特性收益 A）</param>
    /// <param name="defaultParamValues">复算字典基底（页面的 ParamValuesDictionary 拷贝）</param>
    /// <returns>(总收益 Score, 评级 GradeText)</returns>
    public static (double score, string grade) ComputeScore(
        SyTreatRecord record,
        double pageTankRatio,
        Dictionary<string, string> lingyunNameToCode,
        Dictionary<string, string> neigongNameToCode,
        Dictionary<string, ParamValue> defaultParamValues)
    {
        // 以页面参数字典为基底（含 ST_* 输入与默认值），避免修改页面共享字典
        var valueDic = new Dictionary<string, ParamValue>(defaultParamValues ?? new());

        // 1. 装填记录的词条数值（按 Code 覆盖基底字典中对应词条的 NumberValue）
        foreach ((string code, double value) in record.InputValues ?? new())
        {
            valueDic[code] = new() { NumberValue = value, NumberMode = true };
        }

        // 2. 同步疗承比到字典（ST_TankRatio），必须先于特性/灵韵计算（固垒依赖疗承比）
        valueDic["ST_TankRatio"] = new() { NumberValue = pageTankRatio, NumberMode = true };

        // 兼容旧记录迁移：未填 NeiGongSelection/HasLingYun 时从 LingYunSelection 推导
        string neigongSel = record.NeiGongSelection;
        string hasLingYun = record.HasLingYun;
        if (string.IsNullOrEmpty(neigongSel))
        {
            neigongSel = !string.IsNullOrEmpty(record.LingYunSelection) ? record.LingYunSelection : "无";
        }
        if (string.IsNullOrEmpty(hasLingYun))
        {
            hasLingYun = !string.IsNullOrEmpty(record.LingYunSelection) && record.LingYunSelection != "无" ? "有" : "无";
        }

        // 3. 计算特性收益 A：内功名称非「无」→ 查「内功特性收益」表(RF_TS_*)
        double teSeGain = 0;
        if (neigongSel != "无"
            && neigongNameToCode != null
            && neigongNameToCode.TryGetValue(neigongSel, out var tsCode))
        {
            var tsResult = TreatUtility.CalculateRaw(new List<string> { tsCode }, valueDic);
            if (tsResult.TryGetValue(tsCode, out var tsObj) && tsObj is double tsVal)
            {
                teSeGain = tsVal;
            }
        }
        valueDic["ST_PVP_TeSe"] = new() { NumberValue = teSeGain, NumberMode = true };

        // 4. 计算灵韵收益 B：灵韵=「有」且内功名称非「无」→ 按内功名查「灵韵收益」表(RF_LY_*)
        double lingyunGain = 0;
        if (hasLingYun == "有"
            && neigongSel != "无"
            && lingyunNameToCode != null
            && lingyunNameToCode.TryGetValue(neigongSel, out var lyCode))
        {
            var lyResult = TreatUtility.CalculateRaw(new List<string> { lyCode }, valueDic);
            if (lyResult.TryGetValue(lyCode, out var lyObj) && lyObj is double lyVal)
            {
                lingyunGain = lyVal;
            }
        }
        valueDic["ST_PVP_LingYun"] = new() { NumberValue = lingyunGain, NumberMode = true };

        // 5. 计算总收益与评级（公式已含 ST_PVP_TeSe）
        var rawResults = TreatUtility.CalculateRaw(PvpCodes, valueDic);

        double score = 0;
        if (rawResults.TryGetValue("RF_TotalGain", out var totalObj) && totalObj is double total)
        {
            score = total;
        }

        string grade = "";
        if (rawResults.TryGetValue("RF_Grade", out var gradeObj))
        {
            grade = gradeObj?.ToString() ?? "";
        }

        return (score, grade);
    }

    /// <summary>
    /// 仅复算记录的内功特性收益 A（供弹窗实时展示，无需重算总收益/评级）。
    /// 流程与 <see cref="ComputeScore"/> 的特性收益 A 段一致：装字典 → 同步疗承比 → 查 RF_TS_*。
    /// </summary>
    /// <param name="record">被复算的内功记录（读取 InputValues/NeiGongSelection；旧记录回退 LingYunSelection）</param>
    /// <param name="pageTankRatio">页面当前疗承比（固垒特性依赖）</param>
    /// <param name="neigongNameToCode">内功名称 -> RF_TS_* Code 映射</param>
    /// <param name="defaultParamValues">复算字典基底（页面的 ParamValuesDictionary 拷贝）</param>
    /// <returns>内功特性收益 A（未选内功或查无对应收益时为 0）</returns>
    public static double ComputeTeSeGain(
        SyTreatRecord record,
        double pageTankRatio,
        Dictionary<string, string> neigongNameToCode,
        Dictionary<string, ParamValue> defaultParamValues)
    {
        // 以页面参数字典为基底（含 ST_* 输入与默认值），避免修改页面共享字典
        var valueDic = new Dictionary<string, ParamValue>(defaultParamValues ?? new());

        // 1. 装填记录的词条数值（按 Code 覆盖基底字典中对应词条的 NumberValue）
        foreach ((string code, double value) in record.InputValues ?? new())
        {
            valueDic[code] = new() { NumberValue = value, NumberMode = true };
        }

        // 2. 同步疗承比到字典（ST_TankRatio），固垒特性依赖疗承比
        valueDic["ST_TankRatio"] = new() { NumberValue = pageTankRatio, NumberMode = true };

        // 兼容旧记录迁移：未填 NeiGongSelection 时从 LingYunSelection 推导
        string neigongSel = record.NeiGongSelection;
        if (string.IsNullOrEmpty(neigongSel))
        {
            neigongSel = !string.IsNullOrEmpty(record.LingYunSelection) ? record.LingYunSelection : "无";
        }

        // 3. 内功名称非「无」→ 查「内功特性收益」表(RF_TS_*)
        if (neigongSel != "无"
            && neigongNameToCode != null
            && neigongNameToCode.TryGetValue(neigongSel, out var tsCode))
        {
            var tsResult = TreatUtility.CalculateRaw(new List<string> { tsCode }, valueDic);
            if (tsResult.TryGetValue(tsCode, out var tsObj) && tsObj is double tsVal)
            {
                return tsVal;
            }
        }

        return 0;
    }
}
