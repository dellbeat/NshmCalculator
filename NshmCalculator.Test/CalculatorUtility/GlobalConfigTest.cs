using System.Text.Json;
using NshmCalculator.Shared.Models.BaseModel;
using NshmCalculator.Shared.Models.CalculatorModel.CalculatorConfig;

namespace NshmCalculator.Test.CalculatorUtility;

public class GlobalConfigTest
{
    private const string VersionFilePath = "data/version.json";
    private const string PveCode = "pve";
    private const string SyTreatCode = "syTreat";
    private const string SfCode = "sf";
    private const string updateCode = "update";
    private AppVersionInfo _version;

    [SetUp]
    public void Init()
    {
        _version = JsonSerializer.Deserialize<AppVersionInfo>(File.ReadAllText(VersionFilePath));
    }

    [Test]
    public void PveConfigVersionNumberTest()
    {
        PveConfig pveConfig = null;
        string path = _version.ConfigPathInfo[PveCode].Replace("..", ".");
        Assert.That(File.Exists(path), Is.True, "无法查找到PVE收益计算器的配置文件");
        Assert.That(File.ReadAllLines(path).Length == 1, Is.True, "PVE收益计算器JSON配置未压缩");
        try
        {
            pveConfig = JsonSerializer.Deserialize<PveConfig>(File.ReadAllText(path));
        }
        catch (Exception e)
        {
            Assert.Fail($"PVE收益计算器-配置解析出现问题:{e.Message}");
        }

        Assert.That(pveConfig.InternalVersion == _version.ConfigVersionInfo[PveCode], Is.True, "PVE收益计算器版本号校验不一致");
        Assert.Pass("PVE收益计算器版本号校验通过");
    }

    [Test]
    public void SyTreatConfigVersionNumberTest()
    {
        SyTreatConfig syTreatConfig = null;
        string path = _version.ConfigPathInfo[SyTreatCode].Replace("..", ".");
        Assert.That(File.Exists(path), Is.True, "无法查找到素问治疗计算器的配置文件");
        Assert.That(File.ReadAllLines(path).Length == 1, Is.True, "素问治疗计算器JSON配置未压缩");
        try
        {
            syTreatConfig = JsonSerializer.Deserialize<SyTreatConfig>(File.ReadAllText(path));
        }
        catch (Exception e)
        {
            Assert.Fail($"素问治疗计算器-配置解析出现问题:{e.Message}");
        }

        Assert.That(syTreatConfig.InternalVersion == _version.ConfigVersionInfo[SyTreatCode], Is.True, "素问治疗计算器版本号校验不一致");
        Assert.Pass("素问治疗计算器版本号校验通过");
    }

    [Test]
    public void SfConfigVersionNumberTest()
    {
        OccupationConfig sfConfig = null;
        string path = _version.ConfigPathInfo[SfCode].Replace("..", ".");
        Assert.That(File.Exists(path), Is.True, "无法查找到身份收益计算器的配置文件");
        Assert.That(File.ReadAllLines(path).Length == 1, Is.True, "身份收益计算器JSON配置未压缩");
        try
        {
            sfConfig = JsonSerializer.Deserialize<OccupationConfig>(File.ReadAllText(path));
        }
        catch (Exception e)
        {
            Assert.Fail($"身份收益计算器-配置解析出现问题:{e.Message}");
        }

        Assert.That(sfConfig.InternalVersion == _version.ConfigVersionInfo[SfCode], Is.True, "身份收益计算器版本号校验不一致");
        Assert.Pass("身份收益计算器版本号校验通过");
    }

    [Test]
    public void UpdateLogVersionNumberTest()
    {
        UpdateLog firstLog = null;
        string path = _version.ConfigPathInfo[updateCode].Replace("..", ".");
        Assert.That(File.Exists(path), Is.True, "无法查找到更新日志的配置文件");
        try
        {
            firstLog = JsonSerializer.Deserialize<UpdateLog[]>(File.ReadAllText(path)).First();
        }
        catch (Exception e)
        {
            Assert.Fail($"更新日志-配置解析出现问题:{e.Message}");
        }

        string versionStr = _version.ConfigVersionInfo[updateCode].ToString();
        DateTime logTime = DateTime.Parse(firstLog.DateTime);
        Assert.That(
            logTime.Year.ToString() == versionStr.Substring(0, 4) && logTime.Month.ToString() == versionStr.Substring(4, 2).TrimStart('0') &&
            logTime.Day.ToString() == versionStr.Substring(6, 2).TrimStart('0'), Is.True, "更新日志-最新的日志更新时间与版本号无法对应");
        Assert.Pass("更新日志配置版本号校验通过");
    }
}