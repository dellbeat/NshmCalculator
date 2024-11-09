using Newtonsoft.Json;
using NshmCalculator.Shared;
using NshmCalculator.Shared.Models.BaseModel;
using NshmCalculator.Shared.Models.CalculatorModel;

namespace NshmCalculator.Test.CalculatorUtility;

[TestFixture]
public partial class TreatUtilityTest
{
    [Test, TestCaseSource(nameof(TreatTestData))]
    public void TreatChangeTest(string version, TreatInfo baseInfo, TreatInfo newInfo, (double, double) expectedValue)
    {
        string jsonText = File.ReadAllText(JsonFilePath);
        SyTreatConfig[] configArray = JsonConvert.DeserializeObject<SyTreatConfig[]>(jsonText);

        if (configArray == null || configArray.Length == 0)
        {
            Assert.Fail("无有效配置数据，请检查");
        }

        var config = configArray.FirstOrDefault(s => s.Version == version);

        if (config == null)
        {
            Assert.Fail("未找到对应版本配置，请检查");
        }

        var calculateValue = TreatUtility.Compare(baseInfo, newInfo, config);

        Assert.That(Math.Abs(calculateValue.Item1 - expectedValue.Item1), Is.LessThan(DoublePrecision));
        Assert.That(Math.Abs(calculateValue.Item2 - expectedValue.Item2), Is.LessThan(DoublePrecision));
        Assert.Pass("治疗对比测试通过");
    }
}