using NshmCalculator.Shared.Models.BaseModel;
using NshmCalculator.Shared.Models.CalculatorModel;
using NshmCalculator.Shared.Models.CalculatorModel.CalculatorConfig;

namespace NshmCalculator.Shared;

public static class TreatUtility
{
    /// <summary>
    /// 计算治疗量与会心率（对应4.1.1表格I10/J10公式）
    /// <para>会心率 = 1/(1+e^(1−会心/CriticalRating50)) + 额外会心率</para>
    /// <para>奶量   = 治疗强度 × (1 + (会伤−1) × 会心率) × 疗效增益</para>
    /// </summary>
    /// <param name="info">治疗面板信息</param>
    /// <param name="config">治疗计算器配置</param>
    private static void Calculate(TreatInfo info, SyTreatConfig config)
    {
        info.CalculateCriticalHitsRate =
            1.0 / (1.0 + Math.Exp(1 - info.CriticalHits / config.CriticalRating50))
            + info.ExtraCriticalHitsRate / 100.0;
        info.CalculateTreatNum =
            info.TreatIntensity
            * (1 + (info.CriticalDamageRate / 100.0 - 1) * info.CalculateCriticalHitsRate)
            * info.CureGain / 100.0;
    }

    /// <summary>
    /// 计算差异量
    /// </summary>
    /// <param name="baseInfo">基础治疗面板信息</param>
    /// <param name="changeInfo">改动后的治疗面板信息</param>
    /// <param name="config">治疗计算器配置</param>
    /// <returns>治疗增量和收益率</returns>
    public static (double, double) Compare(TreatInfo baseInfo, TreatInfo changeInfo, SyTreatConfig config)
    {
        Calculate(baseInfo, config);
        Calculate(changeInfo, config);
        double dispersion = changeInfo.CalculateTreatNum - baseInfo.CalculateTreatNum;
        double gainRate = dispersion / baseInfo.CalculateTreatNum;

        return (dispersion, gainRate);
    }

    /// <summary>
    /// 计算单组面板的会心率与奶量（供单元测试调用，内部走 Calculate）
    /// </summary>
    public static (double criticalHitsRate, double treatNum) Compute(TreatInfo info, SyTreatConfig config)
    {
        Calculate(info, config);
        return (info.CalculateCriticalHitsRate, info.CalculateTreatNum);
    }

    /// <summary>
    /// 动态属性配置通用方法
    /// </summary>
    /// <param name="fields">属性信息</param>
    /// <returns></returns>
    public static double FieldCalculate(FieldInfo[] fields, double transDefenseRate)
    {
        double result = 0;
        foreach (var fieldInfo in fields)
        {
            for (int i = 0; i < fieldInfo.ExceptRatioList.Count; i++)
            {
                result += fieldInfo.Value * (fieldInfo.PercentMode ? 0.01 : 1) * fieldInfo.MultiCoeList[i] / fieldInfo.ExceptCoeList[i] /
                          (fieldInfo.ExceptRatioList[i] ? transDefenseRate : 1);
            }
        }

        return result;
    }
}