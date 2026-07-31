using System.Text.Json;
using NshmCalculator.Shared;
using NshmCalculator.Shared.Models.CalculatorModel;
using NshmCalculator.Shared.Models.CalculatorModel.CalculatorConfig;
using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Params;

namespace NshmCalculator.Test.CalculatorUtility;

/// <summary>
/// 针对4.1.1奶量/会心率/PVP收益计算的数值回归测试（NCalc 配置驱动版）。
/// 奶量基线取自 4.1.1 表格 I10/J10 的已知样例。
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
        // 初始化 NCalc 计算引擎
        TreatUtility.InitUtilityFromConfig(_config);
    }

    #region 阶段A 基线：奶量/会心率（Logistic 模型）

    /// <summary>
    /// 会心 1534 → 会心率 ≈ 0.4182（4.1.1表格 I10 基线）
    /// </summary>
    [Test]
    public void CriticalRate_LogisticAt1534_MatchesTableBaseline()
    {
        var info = new TreatInfo { TreatIntensity = 0, CriticalHits = 1534, ExtraCriticalHitsRate = 0, CriticalDamageRate = 150, CureGain = 170 };
        var (critRate, _) = TreatUtility.ComputeHeal(info, _config);
        Assert.That(critRate, Is.EqualTo(0.4182).Within(1e-4),
            $"会心1534时会心率应为0.4182，实际{critRate}");
    }

    /// <summary>
    /// 会心 2290 → 会心率 ≈ 0.5（验证 2290 = 50% 临界评分常量，写入 SF_CritRate 公式）
    /// </summary>
    [Test]
    public void CriticalRating50_IsHalfCritPoint()
    {
        var info = new TreatInfo { TreatIntensity = 0, CriticalHits = 2290, ExtraCriticalHitsRate = 0, CriticalDamageRate = 150, CureGain = 170 };
        var (critRate, _) = TreatUtility.ComputeHeal(info, _config);
        Assert.That(critRate, Is.EqualTo(0.5).Within(1e-4),
            $"会心2290时会心率应为0.5，实际{critRate}");
    }

    /// <summary>
    /// 额外会心率作为加项叠加在Logistic输出之上（百分比整数输入，公式内部 /100）
    /// 会心1534 + 额外会心率3(%) → 会心率 ≈ 0.4182 + 0.03 = 0.4482
    /// </summary>
    [Test]
    public void CriticalRate_ExtraRateAdded_OnTopOfLogistic()
    {
        var info = new TreatInfo { TreatIntensity = 0, CriticalHits = 1534, ExtraCriticalHitsRate = 3, CriticalDamageRate = 150, CureGain = 170 };
        var (critRate, _) = TreatUtility.ComputeHeal(info, _config);
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
            CriticalDamageRate = 150,
            ExtraCriticalHitsRate = 0,
            CureGain = 170
        };
        var (_, treatNum) = TreatUtility.ComputeHeal(info, _config);
        Assert.That(treatNum, Is.EqualTo(3083.22).Within(0.01),
            $"奶量应为3083.22，实际{treatNum}");
    }

    /// <summary>
    /// CompareHeal：基础面板 vs +治疗强度100，应返回正增量且收益率为正
    /// </summary>
    [Test]
    public void CompareHeal_PositiveTreatIntensityIncrease_ReturnsPositiveGain()
    {
        var baseInfo = new TreatInfo
        {
            TreatIntensity = 1500, CriticalHits = 1534, CriticalDamageRate = 150, CureGain = 170
        };
        var changeInfo = new TreatInfo
        {
            TreatIntensity = 1600, CriticalHits = 1534, CriticalDamageRate = 150, CureGain = 170
        };
        var (dispersion, gainRate) = TreatUtility.CompareHeal(baseInfo, changeInfo, _config);
        Assert.That(dispersion, Is.GreaterThan(0), "治疗强度+100应带来正增量");
        Assert.That(gainRate, Is.GreaterThan(0), "收益率应为正");
    }

    /// <summary>
    /// 疗效增益线性放大奶量：CureGain翻倍则奶量翻倍
    /// </summary>
    [Test]
    public void TreatNum_ScalesLinearlyWithCureGain()
    {
        var baseInfo = new TreatInfo
        {
            TreatIntensity = 1500, CriticalHits = 1534, CriticalDamageRate = 150, CureGain = 170
        };
        var doubleInfo = new TreatInfo
        {
            TreatIntensity = 1500, CriticalHits = 1534, CriticalDamageRate = 150, CureGain = 340
        };
        var (_, t1) = TreatUtility.ComputeHeal(baseInfo, _config);
        var (_, t2) = TreatUtility.ComputeHeal(doubleInfo, _config);
        Assert.That(t2 / t1, Is.EqualTo(2.0).Within(1e-6),
            "疗效增益翻倍奶量应翻倍");
    }

    #endregion

    #region 阶段B/C 基线：PVP 总收益 K23 与评级

    /// <summary>
    /// PVP 总收益：全 0 输入 + 灵韵 0 + 赛年 0 → K23 应为 0
    /// </summary>
    [Test]
    public void TotalGain_AllZero_EqualsZero()
    {
        var valueDic = BuildZeroPvpValueDic();
        var result = TreatUtility.Calculate(new List<string> { "RF_TotalGain" }, valueDic);
        Assert.That(result["RF_TotalGain"], Is.EqualTo(0).Within(1e-9),
            "全0输入时K23总收益应为0");
    }

    /// <summary>
    /// 评级：总收益 6% → S+（验证 RF_Grade 用 NCalc ifs）
    /// </summary>
    [Test]
    public void Grade_SPlus_WhenTotalGainAbove55Percent()
    {
        var valueDic = BuildZeroPvpValueDic();
        // 通过灵韵收益直接抬到 6%（>5.5% → S+）
        valueDic["ST_PVP_LingYun"] = new() { NumberValue = 0.06, NumberMode = true };
        var rawResult = TreatUtility.CalculateRaw(new List<string> { "RF_Grade" }, valueDic);
        Assert.That(rawResult["RF_Grade"]?.ToString(), Does.Contain("S+"),
            $"总收益6%应评级S+，实际{rawResult["RF_Grade"]}");
    }

    /// <summary>
    /// 评级：总收益 4.2% → B（4.0% ≤ x &lt; 4.5%）
    /// </summary>
    [Test]
    public void Grade_B_WhenTotalGainBetween40And45Percent()
    {
        var valueDic = BuildZeroPvpValueDic();
        valueDic["ST_PVP_LingYun"] = new() { NumberValue = 0.042, NumberMode = true };
        var rawResult = TreatUtility.CalculateRaw(new List<string> { "RF_Grade" }, valueDic);
        Assert.That(rawResult["RF_Grade"]?.ToString(), Does.Contain("B"),
            $"总收益4.2%应评级B，实际{rawResult["RF_Grade"]}");
    }

    /// <summary>
    /// 评级：总收益 3% → C（&lt; 4.0%）
    /// </summary>
    [Test]
    public void Grade_C_WhenTotalGainBelow40Percent()
    {
        var valueDic = BuildZeroPvpValueDic();
        valueDic["ST_PVP_LingYun"] = new() { NumberValue = 0.03, NumberMode = true };
        var rawResult = TreatUtility.CalculateRaw(new List<string> { "RF_Grade" }, valueDic);
        Assert.That(rawResult["RF_Grade"]?.ToString(), Does.Contain("C"),
            $"总收益3%应评级C，实际{rawResult["RF_Grade"]}");
    }

    /// <summary>
    /// 构造全 0 的 PVP 参数字典（含疗承比默认1.3）
    /// </summary>
    private Dictionary<string, ParamValue> BuildZeroPvpValueDic()
    {
        var codes = new[]
        {
            "ST_PVP_Qi", "ST_PVP_Atk", "ST_PVP_AtkMax", "ST_PVP_AtkMin", "ST_PVP_DefBreak",
            "ST_PVP_Crit", "ST_PVP_Heal", "ST_PVP_Gen", "ST_PVP_Agi", "ST_PVP_HP",
            "ST_PVP_Sta", "ST_PVP_Def", "ST_PVP_DefN", "ST_PVP_DefW", "ST_PVP_ACritW",
            "ST_PVP_ACritN", "ST_PVP_SchoolDef", "ST_PVP_ACrit", "ST_PVP_Restrain",
            "ST_PVP_Resist", "ST_PVP_CritDmg", "ST_PVP_CritDef", "ST_PVP_SaiNian", "ST_PVP_LingYun",
            "ST_PVP_TeSe",
        };
        var dic = new Dictionary<string, ParamValue>();
        foreach (var code in codes)
        {
            dic[code] = new ParamValue { NumberValue = 0, NumberMode = true };
        }
        dic["ST_TankRatio"] = new ParamValue { NumberValue = 1.3, NumberMode = true };
        return dic;
    }

    #endregion
}
