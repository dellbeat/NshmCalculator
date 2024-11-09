using NshmCalculator.Shared.Models.CalculatorModel;

namespace NshmCalculator.Test.CalculatorUtility;

public partial class TreatUtilityTest
{
    private const string JsonFilePath = "data/syTreatConfig.json";
    private const double DoublePrecision = 1e-6;

    private static IEnumerable<object> TreatTestData
    {
        get
        {
            yield return new object[]
            {
                "2.2.1.2",
                new TreatInfo
                {
                    Attack = 13000, TreatIntensity = 12500, CriticalHits = 3400,
                    CriticalDamageRate = 159
                },
                new TreatInfo
                {
                    Attack = 13500, TreatIntensity = 12600, CriticalHits = 3400,
                    CriticalDamageRate = 159
                },
                (491.157592, 0.017480)
            };
            yield return new object[]
            {
                "2.2.1.2",
                new TreatInfo
                {
                    Attack = 13000, TreatIntensity = 12500, CriticalHits = 3400,
                    CriticalDamageRate = 159, ZtCriticalHitsRate = 2
                },
                new TreatInfo
                {
                    Attack = 14500, TreatIntensity = 12860, CriticalHits = 3700,
                    CriticalDamageRate = 162.5, ZtCriticalHitsRate = 2, ExtraCriticalHitsRate = 0.01
                },
                (1982.898629, 0.070198)
            };
            yield return new object[]
            {
                "2.2.1.2",
                new TreatInfo
                {
                    Attack = 10000, TreatIntensity = 14000, CriticalHits = 3500,
                    CriticalDamageRate = 163
                },
                new TreatInfo
                {
                    Attack = 11500, TreatIntensity = 14360, CriticalHits = 3800,
                    CriticalDamageRate = 166.5, ExtraCriticalHitsRate = 0.01
                },
                (1991.506430, 0.071210)
            };
        }
    }
}