using System.Text.Json;
using NshmCalculator.Shared;
using NshmCalculator.Shared.Models.CalculatorModel;
using NshmCalculator.Shared.Models.CalculatorModel.CalculatorConfig;

namespace NshmCalculator.Test.CalculatorUtility;

/// <summary>
/// 针对4.1.1奶量/会心率计算（Logistic会心率 + 纯治疗强度基底 + 疗效增益）的数值回归测试。
/// 基线值取自 4.1.1 表格 I10/J10 的已知样例。
/// </summary>
[TestFixture]
public class TreatUtilityTest
{
    private const string JsonFilePath = "data/config_syTreat.json";
    private SyTreatConfig _config = null!;

    [SetUp]
    public void Init()
    {
        _config = JsonSerializer.Deserialize<SyTreatConfig>(File.ReadAllText(JsonFilePath))!;
    }

    /// <summary>
    /// 会心 1534 → 会心率 ≈ 0.4182（4.1.1表格 I10 基线）
    /// </summary>
    [Test]
    public void CriticalRate_LogisticAt1534_MatchesTableBaseline()
    {
        var info = new TreatInfo { CriticalHits = 1534, ExtraCriticalHitsRate = 0 };
        var (critRate, _) = TreatUtility.Compute(info, _config);
        Assert.That(critRate, Is.EqualTo(0.4182).Within(1e-4),
            $"会心1534时会心率应为0.4182，实际{critRate}");
    }

    /// <summary>
    /// 会心 2290 → 会心率 ≈ 0.5（验证 2290 = 50% 临界评分常量）
    /// </summary>
    [Test]
    public void CriticalRating50_IsHalfCritPoint()
    {
        Assert.That(_config.CriticalRating50, Is.EqualTo(2290),
            "CriticalRating50 应为 2290");
        var info = new TreatInfo { CriticalHits = 2290, ExtraCriticalHitsRate = 0 };
        var (critRate, _) = TreatUtility.Compute(info, _config);
        Assert.That(critRate, Is.EqualTo(0.5).Within(1e-4),
            $"会心2290时会心率应为0.5，实际{critRate}");
    }

    /// <summary>
    /// 额外会心率作为加项叠加在Logistic输出之上（会伤百分比形式输入，内部/100）
    /// 会心1534 + 额外会心率3(%) → 会心率 ≈ 0.4182 + 0.03 = 0.4482
    /// </summary>
    [Test]
    public void CriticalRate_ExtraRateAdded_OnTopOfLogistic()
    {
        var info = new TreatInfo { CriticalHits = 1534, ExtraCriticalHitsRate = 3 };
        var (critRate, _) = TreatUtility.Compute(info, _config);
        Assert.That(critRate, Is.EqualTo(0.4482).Within(1e-4),
            $"额外会心率3%应叠加为0.4482，实际{critRate}");
    }

    /// <summary>
    /// 治疗强度1500、会伤150(%)、会心1534、疗效增益170(%) → 奶量 ≈ 3083.22（4.1.1表格 J10 基线）
    /// </summary>
    [Test]
    public void TreatNum_TableBaseline_Matches3083()
    {
        var info = new TreatInfo
        {
            TreatIntensity = 1500,
            CriticalHits = 1534,
            CriticalDamageRate = 150, // 百分比形式
            ExtraCriticalHitsRate = 0,
            CureGain = 170 // 百分比形式，即1.7倍
        };
        var (_, treatNum) = TreatUtility.Compute(info, _config);
        Assert.That(treatNum, Is.EqualTo(3083.22).Within(0.01),
            $"奶量应为3083.22，实际{treatNum}");
    }

    /// <summary>
    /// Compare：基础面板 vs +治疗强度100，应返回正增量且收益率为正
    /// </summary>
    [Test]
    public void Compare_PositiveTreatIntensityIncrease_ReturnsPositiveGain()
    {
        var baseInfo = new TreatInfo
        {
            TreatIntensity = 1500,
            CriticalHits = 1534,
            CriticalDamageRate = 150,
            CureGain = 170
        };
        var changeInfo = new TreatInfo
        {
            TreatIntensity = 1600,
            CriticalHits = 1534,
            CriticalDamageRate = 150,
            CureGain = 170
        };
        var (dispersion, gainRate) = TreatUtility.Compare(baseInfo, changeInfo, _config);
        Assert.That(dispersion, Is.GreaterThan(0), "治疗强度+100应带来正增量");
        Assert.That(gainRate, Is.GreaterThan(0), "收益率应为正");
    }

    /// <summary>
    /// 疗效增益线性放大奶量：CureGain翻倍则奶量翻倍（在会心率/会伤不变的前提下）
    /// </summary>
    [Test]
    public void TreatNum_ScalesLinearlyWithCureGain()
    {
        var baseInfo = new TreatInfo
        {
            TreatIntensity = 1500,
            CriticalHits = 1534,
            CriticalDamageRate = 150,
            CureGain = 170
        };
        var doubleInfo = new TreatInfo
        {
            TreatIntensity = 1500,
            CriticalHits = 1534,
            CriticalDamageRate = 150,
            CureGain = 340
        };
        var (_, t1) = TreatUtility.Compute(baseInfo, _config);
        var (_, t2) = TreatUtility.Compute(doubleInfo, _config);
        Assert.That(t2 / t1, Is.EqualTo(2.0).Within(1e-6),
            "疗效增益翻倍奶量应翻倍");
    }
}
