using NshmCalculator.Shared;
using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Params;

namespace NshmCalculator.MudClient.Utilities;

/// <summary>
/// 素问治疗计算器内功记录复算工具。
/// 严格复刻 <c>SyTreatCalculator.PvpCalculate</c> 的核心流程（装字典 → 算灵韵 → 算 RF_TotalGain/RF_Grade），
/// 供页面与编辑弹窗共用，保证记录总收益与页面计算结果完全一致。
/// <para>引擎 <see cref="TreatUtility"/> 为静态类，由页面 <c>InitParameters</c> 初始化一次，
/// 弹窗与页面共享同一引擎实例，无需在此重新初始化。</para>
/// </summary>
public static class SyTreatRecordHelper
{
    /// <summary>结果公式代号（与页面 <c>_pvpCodes</c> 保持一致）</summary>
    private static readonly List<string> PvpCodes = new() { "RF_TotalGain", "RF_Grade" };

    /// <summary>
    /// 基于记录的词条数值 + 灵韵选择 + 页面疗承比，调用引擎复算 (总收益, 评级)。
    /// </summary>
    /// <param name="record">被复算的内功记录（仅读取 InputValues/LingYunSelection）</param>
    /// <param name="pageTankRatio">页面当前疗承比（疗承比跟随页面上下文，不存入记录）</param>
    /// <param name="lingyunNameToCode">灵韵名称 -> RF_LY_* Code 映射</param>
    /// <param name="defaultParamValues">复算字典基底（页面的 ParamValuesDictionary 拷贝）</param>
    /// <returns>(总收益 Score, 评级 GradeText)</returns>
    public static (double score, string grade) ComputeScore(
        SyTreatRecord record,
        double pageTankRatio,
        Dictionary<string, string> lingyunNameToCode,
        Dictionary<string, ParamValue> defaultParamValues)
    {
        // 以页面参数字典为基底（含 ST_* 输入与默认值），避免修改页面共享字典
        var valueDic = new Dictionary<string, ParamValue>(defaultParamValues ?? new());

        // 1. 装填记录的词条数值（按 Code 覆盖基底字典中对应词条的 NumberValue）
        foreach ((string code, double value) in record.InputValues ?? new())
        {
            valueDic[code] = new() { NumberValue = value, NumberMode = true };
        }

        // 2. 同步疗承比到字典（ST_TankRatio），必须先于灵韵计算（固垒依赖疗承比）
        valueDic["ST_TankRatio"] = new() { NumberValue = pageTankRatio, NumberMode = true };

        // 3. 计算灵韵下拉框选中项对应的收益值，作为 ST_PVP_LingYun 输入
        double lingyunGain = 0;
        if (!string.IsNullOrEmpty(record.LingYunSelection)
            && record.LingYunSelection != "无"
            && lingyunNameToCode != null
            && lingyunNameToCode.TryGetValue(record.LingYunSelection, out var lyCode))
        {
            var lyResult = TreatUtility.CalculateRaw(new List<string> { lyCode }, valueDic);
            if (lyResult.TryGetValue(lyCode, out var lyObj) && lyObj is double lyVal)
            {
                lingyunGain = lyVal;
            }
        }
        valueDic["ST_PVP_LingYun"] = new() { NumberValue = lingyunGain, NumberMode = true };

        // 4. 计算总收益与评级
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
}
