using NshmCalculator.Shared.Models.BaseModel;
using NshmCalculator.Shared.Models.BaseModel.Enums;
using NshmCalculator.Shared.Models.CalculatorModel;

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
    [TestCase(1350, 1217, 1)]
    [TestCase(1347, 1217, 0.998)]
    [TestCase(1217, 1217, 0.950)]
    [TestCase(949, 1217, 0.837)]
    [TestCase(621, 1217, 0.672)]
    [TestCase(197, 1217, 0.398)]
    public void Hit_Version13_Test(int actualHit, int enemyBlock, double percents)
    {
        double rate =
            Shared.CalculatorUtility.CalculateHitRate(actualHit, enemyBlock, HitCalculateVersion.Version13, true);

        Assert.True(Math.Abs(rate - percents) < 0.001);
        Assert.Pass("1.3命中算法-测试通过");
    }

    /// <summary>
    /// 用于测试1.1的命中公式准确性（数据来源：星语困原计算器输入格挡-命中并查看实际命中率）
    /// </summary>
    /// <param name="actualHit">玩家命中</param>
    /// <param name="enemyBlock">敌方格挡</param>
    /// <param name="percents">期望命中率</param>
    [TestCase(500, 590, 0.891)]
    [TestCase(500, 620, 0.874)]
    [TestCase(900, 920, 0.942)]
    [TestCase(1000, 920, 0.979)]
    [TestCase(1200, 920, 1)]
    public void Hit_Version11_Test(int actualHit, int enemyBlock, double percents)
    {
        double rate =
            Shared.CalculatorUtility.CalculateHitRate(actualHit, enemyBlock, HitCalculateVersion.Version11, true);
        Assert.True(Math.Abs(rate - percents) < 0.001);
        Assert.Pass("1.1命中算法-测试通过");
    }

    /// <summary>
    /// 针对增伤率计算器新增的破盾计算特性进行数据测试
    /// </summary>
    /// <param name="breakAirShield"></param>
    /// <param name="airShield"></param>
    [TestCase(0, 400)]
    [TestCase(500, 500)]
    [TestCase(200, 400)]
    [TestCase(100, 300)]
    public void DamageRate_AirShield_Test(int breakAirShield, int airShield)
    {
        var player = new DamageRateCalculateInfo() 
        {
            BaseAttack = 4000,
            BaseRestraint = 2000,
            BaseElementAttack = 2000,
            BaseBreakDefense = 2000,
            BaseHit = 1000,
            BaseCriticalHits = 1200,
            BaseCriticalRate = 220,
            BaseZtCriticalHitsRate = 3,
            BaseBreakAirShield = breakAirShield,
            IncreaseAttack = 100
        };
        var enemy = new EnemyInfo()
        {
            Block = 950,
            Defense = 5000,
            AntiElementAttack = 200,
            AirShield = 0,
            AntiRestraint = 1000
        };

        double percent1 = Shared.CalculatorUtility
            .CalculateIncreaseRate(player, enemy, player.IncreaseAttack)
            .Item1;

        player.BaseRestraint += Convert.ToInt32(
            Shared.CalculatorUtility.CalculateRemainAirShield(breakAirShield,
                airShield)); //保证输入源按照公式计算后不会出现非整数的情况
        enemy.AirShield = airShield;

        //核心思路为在确定剩余气盾后，对应提升基础克制，判断是否能被抵消，且不影响既有增伤率的计算
        
        double percent2 = Shared.CalculatorUtility
            .CalculateIncreaseRate(player, enemy, player.IncreaseAttack)
            .Item1;

        Assert.True(Math.Abs(percent1 - percent2) < 0.001);

        Assert.Pass("增伤率计算器气盾实装-测试通过");
    }
}