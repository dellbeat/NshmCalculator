using NshmCalculator.Shared.Models.BaseModel.Enums;

namespace NshmCalculator.Test.CalculatorUtility;

[TestFixture]
public class Version13
{
    /// <summary>
    /// 用于测试1.3中命中公式改动（数据来源：阿娟）
    /// </summary>
    /// <param name="actualHit">玩家命中</param>
    /// <param name="enemyBlock">敌方格挡</param>
    /// <param name="percents">期望命中率</param>
    [TestCase(1350, 1217,1)]
    [TestCase(1347, 1217,0.998)]
    [TestCase(1217, 1217,0.950)]
    [TestCase(949, 1217,0.837)]
    [TestCase(621, 1217,0.672)]
    [TestCase(197, 1217,0.398)]
    public void Hit_Version13_Test(int actualHit, int enemyBlock, double percents)
    {
        double rate =
            Shared.CalculatorUtility.CalculateHitRate(actualHit, enemyBlock, HitCalculateVersion.Version13, true);
        
        Assert.True(Math.Abs(rate-percents)<0.001);
        Assert.Pass("1.3命中算法-测试通过");
    }

    /// <summary>
    /// 用于测试1.1的命中公式准确性（数据来源：星语困原计算器输入格挡-命中并查看实际命中率）
    /// </summary>
    /// <param name="actualHit">玩家命中</param>
    /// <param name="enemyBlock">敌方格挡</param>
    /// <param name="percents">期望命中率</param>
    [TestCase(500, 590,0.891)]
    [TestCase(500, 620,0.874)]
    [TestCase(900, 920,0.942)]
    [TestCase(1000, 920,0.979)]
    [TestCase(1200, 920,1)]
    public void Hit_Version11_Test(int actualHit, int enemyBlock, double percents)
    {
        double rate = Shared.CalculatorUtility.CalculateHitRate(actualHit, enemyBlock, HitCalculateVersion.Version11, true);
        Assert.True(Math.Abs(rate - percents) < 0.001);
        Assert.Pass("1.1命中算法-测试通过");
    }
}