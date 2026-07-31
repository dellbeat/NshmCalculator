using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;
using NshmCalculator.Shared;
using NshmCalculator.Shared.Models.CalculatorModel;
using NshmCalculator.Shared.Models.CalculatorModel.CalculatorConfig;
using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Params;

namespace NshmCalculator.Test.CalculatorUtility;

/// <summary>
/// 4.1.1 素鸿治疗计算器数值回归测试(数据驱动版)。
///
/// 数据来源:NshmCalculator.Test/TestData/syTreat_cases.json,由 tools/syTreat_case_gen.py
/// 用 Excel COM 真实重算 4.1.1 表格生成(种子固定,可复现),并转换为 C# 模型坐标:
///   - panel:base_inputs(基线面板)+ delta_inputs(属性变化量)→ 复现 5 个输出
///   - equipment:inputs(PVP 词条 + 疗承比)→ 复现 RF_TotalGain / RF_Grade
///
/// 复现路径对齐 SyTreatCalculator.razor 的实际调用:
///   - 基线 SF_CritRate/SF_TreatNum      = ComputeHeal(baseInfo)
///   - 增量 SF_CritRateDelta             = ComputeHeal(changeInfo).rate
///   - 增量 SF_TreatNumDelta/SF_GainRate = CompareHeal(baseInfo, changeInfo)
///   - 总收益 RF_TotalGain               = Calculate([RF_TotalGain], inputs)
///   - 评级 RF_Grade                     = CalculateRaw([RF_Grade], inputs)
/// </summary>
[TestFixture]
public class TreatUtilityRegressionTest
{
    private const string DataFilePath = "testdata/syTreat_cases.json";
    private const string ConfigFilePath = "data/config_syTreat.json";

    // 浮点容差:Excel COM 与 NCalc 双方均为 IEEE754 double,生成时手算对账偏差 < 1e-9,
    // 这里放宽到 1e-4 留足余量(覆盖 sigmoid 指数计算的微小实现差异)。
    private const double Tolerance = 1e-4;

    private static readonly SyTreatConfig Config = LoadConfig();
    private static readonly RegressionDataset Dataset = LoadDataset();

    [OneTimeSetUp]
    public void InitEngine() => TreatUtility.InitUtilityFromConfig(Config);

    #region TestCaseSource

    public static IEnumerable PanelCases =>
        Dataset.Cases.Where(c => c.Dim == "panel").Select(c => new TestCaseData(c).SetName($"{{m}}({c.Id})"));

    public static IEnumerable EquipmentCases =>
        Dataset.Cases.Where(c => c.Dim == "equipment").Select(c => new TestCaseData(c).SetName($"{{m}}({c.Id})"));

    #endregion

    #region panel 回归:基线 + 增量

    /// <summary>
    /// panel 基线:会心率(SF_CritRate)与奶量(SF_TreatNum)对齐 Excel I10/J10。
    /// </summary>
    [Test, TestCaseSource(nameof(PanelCases))]
    public void Panel_Baseline_MatchesExcel(RegressionCase c)
    {
        var baseInfo = BuildTreatInfo(c.BaseInputs);
        var (rate, num) = TreatUtility.ComputeHeal(baseInfo, Config);

        Assert.Multiple(() =>
        {
            Assert.That(rate, Is.EqualTo(GetExpected(c, "SF_CritRate")).Within(Tolerance),
                $"{c.Id}:基线会心率期望 {GetExpected(c, "SF_CritRate")},实际 {rate}");
            Assert.That(num, Is.EqualTo(GetExpected(c, "SF_TreatNum")).Within(Tolerance),
                $"{c.Id}:基线奶量期望 {GetExpected(c, "SF_TreatNum")},实际 {num}");
        });
    }

    /// <summary>
    /// panel 增量:变化后会心率(SF_CritRateDelta)对齐 Excel I12。
    /// changeInfo = baseInfo 字段 + delta 同名字段(对齐 razor AdditionalCalculate)。
    /// </summary>
    [Test, TestCaseSource(nameof(PanelCases))]
    public void Panel_DeltaCritRate_MatchesExcel(RegressionCase c)
    {
        var baseInfo = BuildTreatInfo(c.BaseInputs);
        var changeInfo = ApplyDelta(baseInfo, c.DeltaInputs);
        var (rate, _) = TreatUtility.ComputeHeal(changeInfo, Config);

        Assert.That(rate, Is.EqualTo(GetExpected(c, "SF_CritRateDelta")).Within(Tolerance),
            $"{c.Id}:变化后会心率期望 {GetExpected(c, "SF_CritRateDelta")},实际 {rate}");
    }

    /// <summary>
    /// panel 增量:奶量增值(SF_TreatNumDelta)与收益率(SF_GainRate)对齐 Excel J12/K12。
    /// </summary>
    [Test, TestCaseSource(nameof(PanelCases))]
    public void Panel_DeltaGain_MatchesExcel(RegressionCase c)
    {
        var baseInfo = BuildTreatInfo(c.BaseInputs);
        var changeInfo = ApplyDelta(baseInfo, c.DeltaInputs);
        var (dispersion, gainRate) = TreatUtility.CompareHeal(baseInfo, changeInfo, Config);

        Assert.Multiple(() =>
        {
            Assert.That(dispersion, Is.EqualTo(GetExpected(c, "SF_TreatNumDelta")).Within(Tolerance),
                $"{c.Id}:奶量增值期望 {GetExpected(c, "SF_TreatNumDelta")},实际 {dispersion}");
            Assert.That(gainRate, Is.EqualTo(GetExpected(c, "SF_GainRate")).Within(Tolerance),
                $"{c.Id}:收益率期望 {GetExpected(c, "SF_GainRate")},实际 {gainRate}");
        });
    }

    #endregion

    #region equipment 回归:总收益 + 评级

    /// <summary>
    /// equipment:RF_TotalGain 对齐 Excel K23(全属性加权和 + 灵韵 + 赛年归一化)。
    /// </summary>
    [Test, TestCaseSource(nameof(EquipmentCases))]
    public void Equipment_TotalGain_MatchesExcel(RegressionCase c)
    {
        var valueDic = ToParamValueDic(c.Inputs);
        var result = TreatUtility.Calculate(new List<string> { "RF_TotalGain" }, valueDic);

        Assert.That(result["RF_TotalGain"], Is.Not.Null, $"{c.Id}:RF_TotalGain 未计算出结果");
        Assert.That(result["RF_TotalGain"]!.Value, Is.EqualTo(GetExpected(c, "RF_TotalGain")).Within(Tolerance),
            $"{c.Id}:总收益期望 {GetExpected(c, "RF_TotalGain")},实际 {result["RF_TotalGain"]!.Value}");
    }

    /// <summary>
    /// equipment:RF_Grade 评级档位对齐 Excel K21。
    /// 注:C# 评级文案带空格(如"S+ 国医圣手"),Excel 数据为全角无空格,故按档位字母前缀比对。
    /// </summary>
    [Test, TestCaseSource(nameof(EquipmentCases))]
    public void Equipment_GradeTier_MatchesExcel(RegressionCase c)
    {
        var valueDic = ToParamValueDic(c.Inputs);
        var rawResult = TreatUtility.CalculateRaw(new List<string> { "RF_Grade" }, valueDic);
        string actualGrade = rawResult["RF_Grade"]?.ToString() ?? "";
        string expectedGrade = GetExpectedString(c, "RF_Grade");

        Assert.That(TierOf(actualGrade), Is.EqualTo(TierOf(expectedGrade)),
            $"{c.Id}:评级期望 [{expectedGrade}] 的档位 {TierOf(expectedGrade)},实际 [{actualGrade}] 的档位 {TierOf(actualGrade)}");
    }

    /// <summary>
    /// equipment:总收益 K23 落在预期目标档位(数据生成时的分档采样验证)。
    /// </summary>
    [Test, TestCaseSource(nameof(EquipmentCases))]
    public void Equipment_TotalGainHitsTargetTier(RegressionCase c)
    {
        // target_tier 是生成时的目标档位;无此字段的用例跳过(向前兼容)
        if (c.TargetTier is null) Assert.Ignore("无 target_tier");
        var valueDic = ToParamValueDic(c.Inputs);
        var result = TreatUtility.Calculate(new List<string> { "RF_TotalGain" }, valueDic);
        double totalGain = result["RF_TotalGain"]!.Value;

        Assert.That(TierOfGain(totalGain), Is.EqualTo(c.TargetTier),
            $"{c.Id}:总收益 {totalGain:P4} 应落在档位 {c.TargetTier},实际 {TierOfGain(totalGain)}");
    }

    #endregion

    #region 辅助

    private static SyTreatConfig LoadConfig()
    {
        // 静态字段初始化器里不能 await,直接同步读
        var config = JsonSerializer.Deserialize<SyTreatConfig>(File.ReadAllText(ConfigFilePath))
                     ?? throw new InvalidOperationException("无法加载 config_syTreat.json");
        TreatUtility.InitUtilityFromConfig(config);
        return config;
    }

    private static RegressionDataset LoadDataset()
    {
        var opts = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
        return JsonSerializer.Deserialize<RegressionDataset>(File.ReadAllText(DataFilePath), opts)
               ?? throw new InvalidOperationException("无法加载回归数据集");
    }

    /// <summary>把 base_inputs(数值字典)转为 TreatInfo。</summary>
    private static TreatInfo BuildTreatInfo(Dictionary<string, double> inputs) => new()
    {
        TreatIntensity = (int)inputs.GetValueOrDefault("ST_Heal"),
        CriticalHits = (int)inputs.GetValueOrDefault("ST_Crit"),
        ExtraCriticalHitsRate = inputs.GetValueOrDefault("ST_ExtraRate"),
        CriticalDamageRate = inputs.GetValueOrDefault("ST_CritDmg"),
        CureGain = inputs.GetValueOrDefault("ST_CureGain"),
    };

    /// <summary>把 delta 叠加到 base 上,生成 changeInfo(对齐 razor AdditionalCalculate)。</summary>
    private static TreatInfo ApplyDelta(TreatInfo baseInfo, Dictionary<string, double> delta) => new()
    {
        TreatIntensity = baseInfo.TreatIntensity + (int)delta.GetValueOrDefault("ST_Heal"),
        CriticalHits = baseInfo.CriticalHits + (int)delta.GetValueOrDefault("ST_Crit"),
        ExtraCriticalHitsRate = baseInfo.ExtraCriticalHitsRate + delta.GetValueOrDefault("ST_ExtraRate"),
        CriticalDamageRate = baseInfo.CriticalDamageRate + delta.GetValueOrDefault("ST_CritDmg"),
        CureGain = baseInfo.CureGain + delta.GetValueOrDefault("ST_CureGain"),
    };

    /// <summary>
    /// equipment 的数值字典 → ParamValue 字典(NumberMode=true)。
    /// 以配置 DefaultParamValues 为基底（与页面 ParamValuesDictionary 初始化一致），
    /// 再用用例输入覆盖；确保 RF_TotalGain 引用的非用例直供参数（如 ST_PVP_TeSe、ST_PVP_LingYun）
    /// 缺省为 0，而非 NCalc 未绑定导致的求值异常。
    /// </summary>
    private static Dictionary<string, ParamValue> ToParamValueDic(Dictionary<string, double> inputs)
    {
        var dic = new Dictionary<string, ParamValue>();
        foreach ((string k, var v) in Config.DefaultParamValues ?? new())
        {
            dic[k] = new ParamValue { NumberValue = v.NumberValue, NumberMode = true };
        }
        foreach ((string k, double v) in inputs)
        {
            dic[k] = new ParamValue { NumberValue = v, NumberMode = true };
        }
        return dic;
    }

    private static double GetExpected(RegressionCase c, string code)
        => c.Expected.TryGetValue(code, out var v) && v is JsonElement je
            ? je.GetDouble()
            : throw new InvalidOperationException($"{c.Id}:缺少期望输出 {code}");

    private static string GetExpectedString(RegressionCase c, string code)
        => c.Expected.TryGetValue(code, out var v) && v is JsonElement je
            ? je.GetString() ?? ""
            : throw new InvalidOperationException($"{c.Id}:缺少期望输出 {code}");

    /// <summary>从评级文案提取档位字母(S+/S/A/B/C)。</summary>
    private static string TierOf(string gradeText)
    {
        string t = gradeText.Trim();
        if (t.StartsWith("S+")) return "S+";
        if (t.StartsWith("S")) return "S";
        if (t.StartsWith("A")) return "A";
        if (t.StartsWith("B")) return "B";
        return t.StartsWith("C") ? "C" : "?";
    }

    /// <summary>按总收益数值判定档位(与 RF_Grade 公式阈值一致)。</summary>
    private static string TierOfGain(double totalGain)
    {
        if (totalGain >= 0.055) return "S+";
        if (totalGain >= 0.05) return "S";
        if (totalGain >= 0.045) return "A";
        if (totalGain >= 0.04) return "B";
        return "C";
    }

    #endregion
}

#region 数据集 DTO

public class RegressionDataset
{
    [JsonPropertyName("schema_version")] public int SchemaVersion { get; set; }
    [JsonPropertyName("format")] public string Format { get; set; } = "";
    [JsonPropertyName("cases")] public List<RegressionCase> Cases { get; set; } = new();
}

public class RegressionCase
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
    [JsonPropertyName("dim")] public string Dim { get; set; } = "";

    [JsonPropertyName("base_inputs")]
    public Dictionary<string, double> BaseInputs { get; set; } = new();

    [JsonPropertyName("delta_inputs")]
    public Dictionary<string, double> DeltaInputs { get; set; } = new();

    [JsonPropertyName("inputs")]
    public Dictionary<string, double> Inputs { get; set; } = new();

    [JsonPropertyName("expected")]
    public Dictionary<string, object> Expected { get; set; } = new();

    [JsonPropertyName("target_tier")]
    public string? TargetTier { get; set; }
}

#endregion
