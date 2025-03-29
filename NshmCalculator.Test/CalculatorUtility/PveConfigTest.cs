using System.Text.Json;
using NshmCalculator.Shared.Models.CalculatorModel;
using NshmCalculator.Shared.Models.CalculatorModel.CalculatorConfig;
using NshmCalculator.Shared.Models.CalculatorModel.Enums;
using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Params;

namespace NshmCalculator.Test.CalculatorUtility;

/// <summary>
/// 针对Pve计算器配置编写的校验测试类
/// </summary>
[TestFixture]
public class PveConfigTest
{
    private const string JsonFilePath = "data/config_pve.json";
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
            $"PVE配置文件({_config.InternalVersion}),中间公式{_config.InternalFormulas.Length}条,结果公式{_config.ResultFormulas.Length}条,选项{_config.FrontParamInfoArray.Length}个,分类{_config.CategoryArray.Length}个,特殊规则选项{_config.FrontParamInfoArray.Count(s => s.Rule != null)}个,特殊规则中间公式{_config.InternalFormulas.Count(s => s.Rule is { Count: > 0 })}个");
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
    public void ParamSpecialRuleCheckTest(FrontParamInfo param)
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
                case ParamRuleMode.Invalid:
                    Assert.Fail($"{param.Code}-不能将模式赋值为非法，请确认是否在Rule中显式赋值为合法值");
                    break;
                case ParamRuleMode.CompareOptions:
                    Assert.That(rule.CustomDic is { Count: > 0 }, Is.True, $"{param.Code}-比对选项模式-没有需要对比的内容");
                    Assert.That(_config.FrontParamInfoArray.Any(s => s.Code == rule.FrontParamCode), Is.True,
                        $"{param.Code}-比对选项模式-未指定前置选项代码或需要修改选项的代码");
                    break;
                case ParamRuleMode.RemoveSameOptions:
                    Assert.That(rule.CustomDic is { Count: > 0 }, Is.True, $"{param.Code}-删除选项模式-没有需要对比的内容");
                    Assert.That(_config.FrontParamInfoArray.Any(s => s.Code == rule.FrontParamCode), Is.True,
                        $"{param.Code}-删除选项模式-未指定前置选项代码或需要修改选项的代码");
                    break;
                case ParamRuleMode.ShareOptions:
                    Assert.That(rule.CustomDic.Count is 2 or 4, Is.True, $"{param.Code}-共享选项模式-参数表映射数量不为2/4");
                    Assert.That(rule.CustomDic.ContainsKey("relateMode") && rule.CustomDic.ContainsKey("groupCode"), Is.True,
                        $"{param.Code}-共享选项模式-未标识是否为关联/未标记组代号");
                    Assert.That(!string.IsNullOrEmpty(rule.CustomDic["groupCode"]), Is.True, $"{param.Code}-共享选项模式-组代号不能为空");
                    Assert.That(
                        rule.CustomDic.Count == 2 || rule.CustomDic.Count == 4 && rule.CustomDic.TryGetValue("emptyStr", out var emptyStr) &&
                        emptyStr != null, Is.True, $"{param.Code}-共享选项模式-关联模式下无有效的空白项标识");
                    break;
                case ParamRuleMode.AssignEnemyData:
                    Assert.That(rule.CustomDic, Has.Count.GreaterThanOrEqualTo(param.Options.Length + 1), $"{param.Code}-关联选项模式-不符合该模式的前置条件");
                    Assert.That(rule.CustomDic.FirstOrDefault().Key is "codes", Is.True, $"{param.Code}-关联选项模式-TextDic首项键值应为codes");
                    string[] relateCodeArray = JsonSerializer.Deserialize<string[]>(rule.CustomDic.FirstOrDefault().Value);
                    Assert.That(relateCodeArray.Length > 0 && relateCodeArray.All(y => _config.FrontParamInfoArray.Any(s => s.Code == y)), Is.True,
                        $"{param.Code}-关联选项模式-关联的数值前端选项无内容/存在无效的选项代号");
                    Assert.That(
                        param.Options.All(s =>
                            rule.CustomDic.ContainsKey(s) &&
                            JsonSerializer.Deserialize<double[]>(rule.CustomDic[s].ToString()) is double[] valueArray &&
                            valueArray.Length == relateCodeArray.Length), $"{param.Code}-关联选项模式，有选项无法找到对应的数值数组或数组元素不符合要求");
                    break;
                case ParamRuleMode.ControlRender:
                    bool status = false;
                    Assert.That(
                        rule.CustomDic.TryGetValue("multConditionMode", out string multModeStr) && bool.TryParse(multModeStr, out status),
                        Is.True, $"{param.Code}-控制渲染模式-多判据模式键值不存在或键值填写有误");
                    Assert.That(
                        status && rule.CustomDic.ContainsKey("multConditionArray") && rule.CustomDic.ContainsKey("multOptionArray") ||
                        !status && rule.CustomDic.ContainsKey("conditionArray") && rule.CustomDic.ContainsKey("optionArray"),
                        Is.True, $"{param.Code}-控制渲染模式-判据和选项键值不存在或不对应");
                    //TODO：试试校验字符串数组？
                    break;
                case ParamRuleMode.RelatedAssignment:
                    Assert.That(rule.CustomDic.ContainsKey("frontCodeArray") && rule.CustomDic.ContainsKey("optionArray"), Is.True,
                        $"{param.Code}-关联赋值模式-不存在关联代码或选项数组");
                    break;
                default:
                    Assert.Fail("不在预期中的枚举值，请确认是否在Rule中显式赋值为合法值");
                    break;
            }

            Assert.Pass($"{param.Code}-特殊规则校验成功");
        }
    }

    /// <summary>
    /// 校验默认值和前端值数量/代号是否匹配
    /// </summary>
    [Test]
    public void DefaultDictionaryCheckTest()
    {
        string[] frontCodeArray = _config.FrontParamInfoArray.Select(s => s.Code).ToArray();
        string[] defaultDicCodeArray = _config.DefaultParamValues.Keys.ToArray();

        Assert.That(frontCodeArray.OrderBy(x => x).SequenceEqual(defaultDicCodeArray.OrderBy(x => x)), Is.True, "前端选项与默认值代号有差异");

        Assert.Pass("前端选项-默认值匹配校验成功");
    }
}