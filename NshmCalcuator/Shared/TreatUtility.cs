using NshmCalculator.Shared.Models.BaseModel;
using NshmCalculator.Shared.Models.CalculatorModel;
using NshmCalculator.Shared.Models.CalculatorModel.CalculatorConfig;

namespace NshmCalculator.Shared;

public static class TreatUtility
{
    /// <summary>
    /// 计算治疗量
    /// </summary>
    /// <param name="info">治疗面板信息</param>
    private static void Calculate(TreatInfo info, SyTreatConfig config = null)
    {
        info.CalculateCriticalHitsRate = (info.CriticalHits * config.CriticalHitsMult + config.CriticalHitsAddition1) /
                                         (info.CriticalHits + config.CriticalHitsAddition2) / 100 +
                                         info.ZtCriticalHitsRate * 0.01 + info.ExtraCriticalHitsRate; //除了100之外全变了
        info.CalculateTreatNum = (info.Attack * 0.6 + info.TreatIntensity + config.TreatIntensityAddition) * 1.106 *
                                 (1 + (info.CriticalDamageRate * 0.01 - 1) / 2 * info.CalculateCriticalHitsRate); //1333系数变化
    }

    /// <summary>
    /// 计算差异量
    /// </summary>
    /// <param name="baseInfo">基础治疗面板信息</param>
    /// <param name="changeInfo">改动后的治疗面板信息</param>
    /// <returns>治疗增量和收益率</returns>
    public static (double, double) Compare(TreatInfo baseInfo, TreatInfo changeInfo, SyTreatConfig config = null)
    {
        double gainRate = 0;
        Calculate(baseInfo, config);
        Calculate(changeInfo, config);
        double dispersion = changeInfo.CalculateTreatNum - baseInfo.CalculateTreatNum;
        gainRate = dispersion / baseInfo.CalculateTreatNum;

        return (dispersion, gainRate);
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