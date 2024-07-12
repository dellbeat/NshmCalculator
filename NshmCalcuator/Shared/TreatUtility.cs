using NshmCalculator.Shared.Models.CalculatorModel;

namespace NshmCalculator.Shared;

public static class TreatUtility
{
    /// <summary>
    /// 计算治疗量
    /// </summary>
    /// <param name="info">治疗面板信息</param>
    private static void Calculate(TreatInfo info)
    {
        info.CalculateCriticalHitsRate = (info.CriticalHits * 103.136842105263 + 2362.61052631569) / (info.CriticalHits + 3248.73684210527) / 100 +
                                         info.ZtCriticalHitsRate * 0.01 + info.ExtraCriticalHitsRate;
        info.CalculateTreatNum = (info.Attack * 0.6 + info.TreatIntensity + 1333) * 1.106 *
                                 (1 + (info.CriticalDamageRate * 0.01 - 1) / 2 * info.CalculateCriticalHitsRate);
    }

    /// <summary>
    /// 计算差异量
    /// </summary>
    /// <param name="baseInfo">基础治疗面板信息</param>
    /// <param name="changeInfo">改动后的治疗面板信息</param>
    /// <returns>治疗增量和收益率</returns>
    public static (double, double) Compare(TreatInfo baseInfo, TreatInfo changeInfo)
    {
        double gainRate = 0;
        Calculate(baseInfo);
        Calculate(changeInfo);
        double dispersion = changeInfo.CalculateTreatNum - baseInfo.CalculateTreatNum;
        gainRate = dispersion / baseInfo.CalculateTreatNum;

        return (dispersion, gainRate);
    }

    /// <summary>
    /// 转换治疗计算
    /// </summary>
    /// <param name="attribution">属性词条数值信息</param>
    /// <returns></returns>
    public static double TransformCalculate(TreatAttribution attribution)
    {
        return attribution.Attack * 0.6 + attribution.Strength * 4.65 * 0.6 + attribution.HalfAttackSum * 0.5 * 0.6 +
               attribution.BreakDefense * 1.0 / 350 * 100 + attribution.ElementAttack * 1.0 / 150 * 100 + attribution.MonsterRestraint * 1.0 / 272 * 100 +
               attribution.Hit * 1.0 / 112 * 100 +
               attribution.IgnoreElementDefense / 114.7 * 100 + attribution.ProfessionRestraint * 1.0 / 320 * 100;
    }
}