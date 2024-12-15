using System.Text.Json;
using NshmCalculator.Shared.Models.CalculatorModel;
using NshmCalculator.Shared.Models.CalculatorModel.CalculatorConfig;
using NshmCalculator.Shared.Models.CalculatorModel.Enums;

namespace NshmCalculator.Test.CalculatorUtility;

/// <summary>
/// 针对Pve计算器配置编写的校验测试类
/// </summary>
[TestFixture]
public class PveConfigTest
{
    private const string JsonFilePath = "data/config_pve.json";
    private const long MinVersionCode = 20240101001;
    private const long MaxVersionCode = 21240101001;
    private const int WarningDays = 60;

    private static IEnumerable<object> _ruleFrontParams
    {
        get
        {
            var config = JsonSerializer.Deserialize<PveConfig>(File.ReadAllText(JsonFilePath));

            var rules = config.FrontParamInfoArray.Where(s => s.Rule != null).ToArray();
            List<object[]> objects = new List<object[]>();
            foreach (var rule in rules)
            {
                objects.Add(new[] { rule });
            }

            return objects;
        }
    }

    private PveConfig? _config;

    [SetUp]
    public void Init()
    {
        var jsonText = File.ReadAllText(JsonFilePath);
        _config = JsonSerializer.Deserialize<PveConfig>(jsonText);
    }

    /// <summary>
    /// 确认文件解析
    /// </summary>
    [Test]
    public void FileValidTest()
    {
        Assert.That(_config != null, Is.True, "文件解析失败");
        Assert.Pass(
            $"PVE配置文件({_config.InternalVersion}),公式{_config.CategoryArray.Length}条,选项{_config.FrontParamInfoArray.Length}个,分类{_config.CategoryArray.Length}个,关联特殊规则选项{_config.FrontParamInfoArray.Count(s => s.Rule != null)}个");
    }

    /// <summary>
    /// 检测版本
    /// </summary>
    [Test]
    public void MinVersionTest()
    {
        Assert.That(_config?.InternalVersion is >= MinVersionCode and <= MaxVersionCode, Is.True, "版本号不在合法版本号范围内");
        Assert.Pass("版本号处于合法版本号范围");
    }

    /// <summary>
    /// 根据系统时间确认版本号是否合理
    /// </summary>
    [Test]
    public void PlausibleVersionTest()
    {
        var timespan = DateTime.Now -
                       new DateTime((int)(_config?.InternalVersion / Math.Pow(10, 7)), (int)(_config?.InternalVersion / Math.Pow(10, 5) % 100),
                           (int)(_config?.InternalVersion / Math.Pow(10, 3) % 100));
        if (timespan.TotalDays > WarningDays)
        {
            Assert.Warn("检测到文件版本号过旧，请确认是否没有实际更新"); //Todo:考虑按文件系统时间判断
        }
        else
        {
            Assert.Pass("版本号合理");
        }
    }

    [Test, TestCaseSource(nameof(_ruleFrontParams))]
    public void RuleCheckTest(FrontParamInfo param)
    {
        if (param.Rule == null)
        {
            Assert.Warn($"{param.Name}（{param.Code}）无对应的规则，测试被跳过");
        }
        else
        {
            var rule = param.Rule;

            switch (rule.Mode)
            {
                case SpecialRuleMode.CompareOptions:
                    Assert.That(rule.TextDic is { Count: > 0 }, Is.True, "比对选项模式-没有需要对比的内容");
                    Assert.That(rule.TextDic.Values.All(s => ((JsonElement)s).ValueKind == JsonValueKind.String), Is.True, "比对选项模式-检测到对比内容中有非字符串");
                    Assert.That(_config.FrontParamInfoArray.Any(s => s.Code == rule.FrontParamCode) &&
                                _config.FrontParamInfoArray.Any(s => s.Code == rule.RelatedParamCode), Is.True, "比对选项模式-未指定前置选项代码或需要修改选项的代码");
                    break;
                case SpecialRuleMode.RemoveSameOptions:
                    Assert.That(rule.TextDic is { Count: > 0 }, Is.True, "删除选项模式-没有需要对比的内容");
                    Assert.That(_config.FrontParamInfoArray.Any(s => s.Code == rule.FrontParamCode), Is.True, "删除选项模式-未指定前置选项代码或需要修改选项的代码");
                    break;
                case SpecialRuleMode.RelatedData:
                    Assert.That(rule.TextDic, Has.Count.GreaterThanOrEqualTo(param.Options.Length + 1), "关联选项模式-不符合该模式的前置条件");
                    Assert.That(rule.TextDic.FirstOrDefault().Key is "codes", Is.True, "关联选项模式-TextDic首项键值应为codes");
                    Assert.That(rule.TextDic.Values.All(s => ((JsonElement)s).ValueKind == JsonValueKind.String), Is.True,
                        "关联选项模式-所有值类型均需要为序列化后的字符串");
                    string[] relateCodeArray = JsonSerializer.Deserialize<string[]>(rule.TextDic.FirstOrDefault().Value.ToString());
                    Assert.That(relateCodeArray.Length > 0 && relateCodeArray.All(y => _config.FrontParamInfoArray.Any(s => s.Code == y)), Is.True,
                        "关联选项模式-关联的数值前端选项无内容/存在无效的选项代号");
                    Assert.That(
                        param.Options.All(s =>
                            rule.TextDic.ContainsKey(s) && JsonSerializer.Deserialize<double[]>(rule.TextDic[s].ToString()) is double[] valueArray &&
                            valueArray.Length == relateCodeArray.Length), "关联选项模式，有选项无法找到对应的数值数组或数组元素不符合要求");
                    break;
            }

            Assert.Pass("特殊规则校验成功");
        }
    }
}