namespace NshmCalculator.Test.CalculatorUtility;

[TestFixture]
public class IndexTest
{
    private const string IndexPath = "data/index.html";

    [Test]
    public void IndexPageCheck()
    {
        string html = File.ReadAllText(IndexPath);
        Assert.That(html.Contains("data-host-url=\"https://us.umami.is\"></script>-->"), Is.False, "未反注释匿名访客统计代码");
        Assert.Pass("统计代码校验通过");
    }
}