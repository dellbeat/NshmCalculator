using NCalc;
using NshmCalculator.Shared.Models.CalculatorModel;
using NshmCalculator.Shared.Models.CalculatorModel.CalculatorConfig;
using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Formula;
using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Params;

namespace NshmCalculator.Shared;

/// <summary>
/// 素问治疗计算器 NCalc 计算引擎（4.1.1 起，对齐 PVE 配置驱动架构，简化版）
/// <para>与 PveUtility 的差异：去掉 Lambda/LinkLambda/Count/复杂分层机制，保留 InternalValueDic 共享池、
/// string.Empty 参数占位约定、两阶段求值（中间公式按 Level 分层 → 结果公式）。</para>
/// </summary>
public static class TreatUtility
{
    /// <summary>
    /// 中间公式代号与表达式实体的映射（按 Level 升序求值，结果写回 InternalValueDic）
    /// </summary>
    private static readonly Dictionary<string, Expression> InternalFormulaExpressionDic = new();

    /// <summary>
    /// 最终结果公式的代号与表达式实体映射
    /// </summary>
    private static readonly Dictionary<string, Expression> ResultFormulaExpressionDic = new();

    /// <summary>
    /// 中间公式分层映射（key=Level，value=该层公式 Code 列表），用于确定求值顺序
    /// </summary>
    private static readonly Dictionary<int, List<string>> FormulaLevelDic = new();

    /// <summary>
    /// 初始化后包含的参数代号列表（来自 FrontParamInfoArray.Code）
    /// </summary>
    private static readonly List<string> ParamCodeList = new();

    /// <summary>
    /// 计算过程中共享的键值对（前端输入 + 中间量结果）
    /// </summary>
    private static readonly Dictionary<string, object> InternalValueDic = new();

    /// <summary>
    /// 从配置初始化计算引擎（清空并重建所有静态字典）
    /// </summary>
    public static void InitUtilityFromConfig(SyTreatConfig config)
    {
        ParamCodeList.Clear();
        FormulaLevelDic.Clear();
        InternalFormulaExpressionDic.Clear();
        ResultFormulaExpressionDic.Clear();
        ParamCodeList.AddRange(config.FrontParamInfoArray.Select(s => s.Code));
        // 补充 DefaultParamValues 的键: 部分参数(如 ST_PVP_LingYun)不作为 FrontParam 渲染,
        // 但仍作为公式输入被引擎引用, 需登记进 ParamCodeList 才能在公式中正确绑定参数占位
        foreach (var key in (config.DefaultParamValues ?? new()).Keys)
        {
            if (!ParamCodeList.Contains(key))
            {
                ParamCodeList.Add(key);
            }
        }
        InitInternalFormulaDic(config.InternalFormulas ?? Array.Empty<PveFormula>());
        InitResultFormulaDic(config.ResultFormulas ?? Array.Empty<PveFormula>());
    }

    /// <summary>
    /// 初始化中间公式表：按 Level 分层，为每个公式登记参数占位（string.Empty）
    /// </summary>
    private static void InitInternalFormulaDic(PveFormula[] array)
    {
        // 收集所有出现过的 Level（>0），升序排列
        var levels = array.Select(s => s.Level).Where(l => l > 0).Distinct().OrderBy(x => x).ToList();

        foreach (int level in levels)
        {
            FormulaLevelDic[level] = new List<string>();
            var levelArray = array.Where(s => s.Level == level).ToArray();
            FormulaLevelDic[level].AddRange(levelArray.Select(s => s.Code));

            foreach (var formula in levelArray)
            {
                Expression exp = new Expression(formula.Formula, ExpressionOptions.IgnoreCaseAtBuiltInFunctions);
                foreach (string param in formula.FormulaParam ?? Array.Empty<string>())
                {
                    // 仅登记"中间公式代号"或"前端参数代号"，避免把字面量误登记为参数
                    if (InternalFormulaExpressionDic.ContainsKey(param) || ParamCodeList.Contains(param))
                    {
                        exp.Parameters[param] = string.Empty;
                    }
                }

                InternalFormulaExpressionDic.Add(formula.Code, exp);
            }
        }
    }

    /// <summary>
    /// 初始化结果公式表：为每个公式登记参数占位
    /// </summary>
    private static void InitResultFormulaDic(PveFormula[] array)
    {
        foreach (var formula in array)
        {
            Expression exp = new Expression(formula.Formula, ExpressionOptions.IgnoreCaseAtBuiltInFunctions);
            foreach (string param in formula.FormulaParam ?? Array.Empty<string>())
            {
                if (InternalFormulaExpressionDic.ContainsKey(param) || ParamCodeList.Contains(param))
                {
                    exp.Parameters[param] = string.Empty;
                }
            }

            ResultFormulaExpressionDic.Add(formula.Code, exp);
        }
    }

    /// <summary>
    /// 计算结果（数值型）
    /// </summary>
    /// <param name="codeList">需要求值的结果公式代号列表</param>
    /// <param name="valueDic">前端参数值字典（key=参数 Code，value=ParamValue）</param>
    /// <returns>结果公式代号 → 计算值（解析失败为 null）</returns>
    public static Dictionary<string, double?> Calculate(List<string> codeList, Dictionary<string, ParamValue> valueDic)
    {
        Dictionary<string, double?> result = new Dictionary<string, double?>();

        // 阶段零：装填前端输入到共享池
        InternalValueDic.Clear();
        foreach ((string key, var value) in valueDic)
        {
            InternalValueDic.Add(key, value.NumberMode ? value.NumberValue : value.StringValue);
        }

        // 阶段一：按 Level 升序求中间公式，结果写回共享池供高层引用
        foreach ((int _, var list) in FormulaLevelDic)
        {
            foreach (string code in list)
            {
                if (InternalFormulaExpressionDic.TryGetValue(code, out var codeExp))
                {
                    // 容错：若该中间公式的任一参数不在当前值池中，跳过（不同场景参数集不同）
                    bool allParamsReady = true;
                    foreach (string paramCode in codeExp.Parameters.Keys)
                    {
                        if (!InternalValueDic.ContainsKey(paramCode))
                        {
                            allParamsReady = false;
                            break;
                        }
                    }

                    if (!allParamsReady)
                    {
                        continue;
                    }

                    try
                    {
                        foreach (string paramCode in codeExp.Parameters.Keys)
                        {
                            codeExp.Parameters[paramCode] = InternalValueDic[paramCode];
                        }

                        InternalValueDic.Add(code, codeExp.Evaluate());
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"中间公式 {code} 求值异常");
                        throw;
                    }
                }
            }
        }

        // 阶段二：求结果公式
        foreach (string code in codeList)
        {
            if (InternalValueDic.TryGetValue(code, out object? resultValue))
            {
                // 该代号已是中间量，直接取
                result.Add(code, Convert.ToDouble(resultValue));
            }
            else if (ResultFormulaExpressionDic.TryGetValue(code, out var formulaExp))
            {
                foreach ((string key, object? _) in formulaExp.Parameters)
                {
                    if (InternalValueDic.TryGetValue(key, out object? internalValue))
                    {
                        formulaExp.Parameters[key] = internalValue;
                    }
                }

                try
                {
                    object evaluated = formulaExp.Evaluate();
                    if (evaluated != null && double.TryParse(evaluated.ToString(), out double value))
                    {
                        result.Add(code, value);
                    }
                    else
                    {
                        // 非数值结果（如评级文本），标记为 null，由调用方用 CalculateRaw 读取字符串
                        result.Add(code, null);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"结果公式 {code} 求值异常");
                    throw;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 便捷方法：基于治疗面板信息（TreatInfo）构造参数字典并求值会心率/奶量。
    /// 供奶量对比 Tab 与单元测试调用。参数 Code 与 config_syTreat.json 约定一致：
    /// 输入 ST_Heal/ST_Crit/ST_ExtraRate/ST_CritDmg/ST_CureGain，输出取中间公式 SF_CritRate/SF_TreatNum。
    /// </summary>
    public static (double criticalHitsRate, double treatNum) ComputeHeal(
        TreatInfo info,
        SyTreatConfig config,
        string codeHealRate = "SF_CritRate",
        string codeTreatNum = "SF_TreatNum")
    {
        var valueDic = new Dictionary<string, ParamValue>
        {
            ["ST_Heal"] = new() { NumberValue = info.TreatIntensity, NumberMode = true },
            ["ST_Crit"] = new() { NumberValue = info.CriticalHits, NumberMode = true },
            ["ST_ExtraRate"] = new() { NumberValue = info.ExtraCriticalHitsRate, NumberMode = true },
            ["ST_CritDmg"] = new() { NumberValue = info.CriticalDamageRate, NumberMode = true },
            ["ST_CureGain"] = new() { NumberValue = info.CureGain, NumberMode = true },
        };

        var result = Calculate(new List<string> { codeHealRate, codeTreatNum }, valueDic);
        double rate = result[codeHealRate] ?? 0;
        double num = result[codeTreatNum] ?? 0;
        return (rate, num);
    }

    /// <summary>
    /// 便捷方法：对比基础面板与变化后面板的奶量差异，返回 (治疗增量, 收益率)。
    /// 变化后面板由基础 + Δ 合成。
    /// </summary>
    public static (double dispersion, double gainRate) CompareHeal(
        TreatInfo baseInfo,
        TreatInfo changeInfo,
        SyTreatConfig config)
    {
        var (_, baseNum) = ComputeHeal(baseInfo, config);
        var (_, changeNum) = ComputeHeal(changeInfo, config);
        double dispersion = changeNum - baseNum;
        double gainRate = baseNum > 0 ? dispersion / baseNum : 0;
        return (dispersion, gainRate);
    }

    /// <summary>
    /// 计算结果（返回原始 object，用于评级等字符串型结果）
    /// </summary>
    public static Dictionary<string, object> CalculateRaw(List<string> codeList, Dictionary<string, ParamValue> valueDic)
    {
        Dictionary<string, object> result = new Dictionary<string, object>();

        InternalValueDic.Clear();
        foreach ((string key, var value) in valueDic)
        {
            InternalValueDic.Add(key, value.NumberMode ? value.NumberValue : value.StringValue);
        }

        foreach ((int _, var list) in FormulaLevelDic)
        {
            foreach (string code in list)
            {
                if (InternalFormulaExpressionDic.TryGetValue(code, out var codeExp))
                {
                    // 容错：若该中间公式的任一参数不在当前值池中，跳过
                    bool allParamsReady = true;
                    foreach (string paramCode in codeExp.Parameters.Keys)
                    {
                        if (!InternalValueDic.ContainsKey(paramCode))
                        {
                            allParamsReady = false;
                            break;
                        }
                    }

                    if (!allParamsReady)
                    {
                        continue;
                    }

                    foreach (string paramCode in codeExp.Parameters.Keys)
                    {
                        codeExp.Parameters[paramCode] = InternalValueDic[paramCode];
                    }

                    InternalValueDic.Add(code, codeExp.Evaluate());
                }
            }
        }

        foreach (string code in codeList)
        {
            if (InternalValueDic.TryGetValue(code, out object? resultValue))
            {
                result.Add(code, resultValue);
            }
            else if (ResultFormulaExpressionDic.TryGetValue(code, out var formulaExp))
            {
                foreach ((string key, object? _) in formulaExp.Parameters)
                {
                    if (InternalValueDic.TryGetValue(key, out object? internalValue))
                    {
                        formulaExp.Parameters[key] = internalValue;
                    }
                }

                result.Add(code, formulaExp.Evaluate());
            }
        }

        return result;
    }
}
