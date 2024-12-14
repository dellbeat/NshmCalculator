using System.Text.Json;
using NshmCalculator.Shared.Models.CalculatorModel.CalculatorConfig;

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
        Assert.True(_config != null, "文件解析失败");
        Assert.Pass("文件解析成功");
    }

    /// <summary>
    /// 检测版本
    /// </summary>
    [Test]
    public void MinVersionTest()
    {
        Assert.IsTrue(_config?.InternalVersion is >= MinVersionCode and <= MaxVersionCode, "版本号不在合法版本号范围内");
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
}