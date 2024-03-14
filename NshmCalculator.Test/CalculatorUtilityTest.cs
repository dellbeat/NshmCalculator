using NshmCalculator.Shared;
using NshmCalculator.Shared.Models.BaseModel;
using NshmCalculator.Shared.Models.CalculatorModel;

namespace NshmCalculator.Test;

public class CalculatorUtilityTest
{
    readonly double rate1 = 0.324 * 923; //系数1
    readonly double rate2 = 0.324; //系数2，暂定为固定值

    [SetUp]
    public void Setup()
    {
    }

    [TestCase(0, 400, 400)]
    [TestCase(500, 0, 500)]
    [TestCase(200, 200, 400)]
    [TestCase(100, 200, 300)]
    public void AirShieldBigTest(int baseBreakShield, int increaseShield, int airShield)
    {
        var calculateInfo = new AttributeCalculateInfo
        {
            BaseBreakAirShield = baseBreakShield,
            IncreaseBreakAirShield = increaseShield
        };
        EnemyInfo enemy = new()
        {
            Block = 630,
            Defense = 1680,
            AntiElementAttack = 60,
            AirShield = airShield
        };

        double damage1 = CalculatorUtility.CalculateBaseDamage(calculateInfo.BaseAttack + calculateInfo.IncreaseAttack,
            calculateInfo.BaseRestraint + calculateInfo.IncreaseRestraint,
            calculateInfo.BaseElementAttack + calculateInfo.IncreaseElementAttack,
            calculateInfo.BaseBreakDefense + calculateInfo.IncreaseBreakDefense, enemy.Defense, enemy.AntiRestraint,
            rate1, rate2, calculateInfo.BaseBreakAirShield + calculateInfo.IncreaseBreakAirShield, enemy.AirShield,
            true);

        double damage2 = CalculatorUtility.CalculateBaseDamage(calculateInfo.BaseAttack + calculateInfo.IncreaseAttack,
            calculateInfo.BaseRestraint + calculateInfo.IncreaseRestraint,
            calculateInfo.BaseElementAttack + calculateInfo.IncreaseElementAttack,
            calculateInfo.BaseBreakDefense + calculateInfo.IncreaseBreakDefense, enemy.Defense, enemy.AntiRestraint,
            rate1, rate2, calculateInfo.BaseBreakAirShield + calculateInfo.IncreaseBreakAirShield, enemy.AirShield,
            false);

        Assert.True(damage1 == damage2);

        Assert.Pass("玩家破盾大于敌方气盾-测试通过");
    }

    [TestCase(0, 30, 100)]
    [TestCase(100, 0, 500)]
    [TestCase(50, 50, 300)]
    public void AirShieldSmallTest(int baseBreakShield, int increaseShield, int airShield)
    {
        var calculateInfo = new AttributeCalculateInfo
        {
            BaseBreakAirShield = baseBreakShield,
            IncreaseBreakAirShield = increaseShield,
            IncreaseRestraint = airShield - 2 * (baseBreakShield + increaseShield)
        };

        EnemyInfo enemy = new()
        {
            Block = 630,
            Defense = 1680,
            AntiElementAttack = 60,
            AirShield = airShield
        };

        double damage1 = CalculatorUtility.CalculateBaseDamage(calculateInfo.BaseAttack + calculateInfo.IncreaseAttack,
            calculateInfo.BaseRestraint + calculateInfo.IncreaseRestraint,
            calculateInfo.BaseElementAttack + calculateInfo.IncreaseElementAttack,
            calculateInfo.BaseBreakDefense + calculateInfo.IncreaseBreakDefense, enemy.Defense, enemy.AntiRestraint,
            rate1, rate2, calculateInfo.BaseBreakAirShield + calculateInfo.IncreaseBreakAirShield, enemy.AirShield,
            true);

        calculateInfo.IncreaseRestraint = 0; //降低剩余气盾，同时降低与之相对的克制数值

        double damage2 = CalculatorUtility.CalculateBaseDamage(calculateInfo.BaseAttack + calculateInfo.IncreaseAttack,
            calculateInfo.BaseRestraint + calculateInfo.IncreaseRestraint,
            calculateInfo.BaseElementAttack + calculateInfo.IncreaseElementAttack,
            calculateInfo.BaseBreakDefense + calculateInfo.IncreaseBreakDefense, enemy.Defense, enemy.AntiRestraint,
            rate1, rate2, calculateInfo.BaseBreakAirShield + calculateInfo.IncreaseBreakAirShield, enemy.AirShield,
            false);

        Assert.True(damage1 == damage2);

        Assert.Pass("玩家破盾小于等于敌方1/3气盾-测试通过");
    }


    [TestCase(100, 100, 400)]
    [TestCase(100, 200, 300)]
    [TestCase(50, 50, 300)]
    public void AirShieldMiddleTest(int baseBreakShield, int increaseShield, int airShield)
    {
        var calculateInfo = new AttributeCalculateInfo
        {
            BaseBreakAirShield = baseBreakShield,
            IncreaseBreakAirShield = increaseShield,
            IncreaseRestraint = (airShield - (baseBreakShield + increaseShield)) / 2
        };

        EnemyInfo enemy = new()
        {
            Block = 630,
            Defense = 1680,
            AntiElementAttack = 60,
            AirShield = airShield
        };

        double damage1 = CalculatorUtility.CalculateBaseDamage(calculateInfo.BaseAttack + calculateInfo.IncreaseAttack,
            calculateInfo.BaseRestraint + calculateInfo.IncreaseRestraint,
            calculateInfo.BaseElementAttack + calculateInfo.IncreaseElementAttack,
            calculateInfo.BaseBreakDefense + calculateInfo.IncreaseBreakDefense, enemy.Defense, enemy.AntiRestraint,
            rate1, rate2, calculateInfo.BaseBreakAirShield + calculateInfo.IncreaseBreakAirShield, enemy.AirShield,
            true);

        calculateInfo.IncreaseRestraint = 0; //降低剩余气盾，同时降低与之相对的克制数值

        double damage2 = CalculatorUtility.CalculateBaseDamage(calculateInfo.BaseAttack + calculateInfo.IncreaseAttack,
            calculateInfo.BaseRestraint + calculateInfo.IncreaseRestraint,
            calculateInfo.BaseElementAttack + calculateInfo.IncreaseElementAttack,
            calculateInfo.BaseBreakDefense + calculateInfo.IncreaseBreakDefense, enemy.Defense, enemy.AntiRestraint,
            rate1, rate2, calculateInfo.BaseBreakAirShield + calculateInfo.IncreaseBreakAirShield, enemy.AirShield,
            false);

        Assert.True(damage1 == damage2);

        Assert.Pass("玩家破盾大于等于敌方1/3气盾值且小于气盾值-测试通过");
    }

    [TestCase(1800,1600,ExpectedResult = 1757.1)]
    [TestCase(1600,1600,ExpectedResult = 1757.1)]
    [TestCase(1000,1600,ExpectedResult = 1508.5)]
    public double DefenseTest(int breakDefense, int enemyDefense)
    {
        var calculateInfo = new AttributeCalculateInfo()
        {
            BaseBreakDefense = breakDefense,
            BaseRestrainedRate = 0,
            BaseZtCriticalHitsRate = 0
        };

        EnemyInfo enemy = new()
        {
            Block = 590,
            Defense = enemyDefense,
            AntiElementAttack = 0,
        };
        
        double damage = CalculatorUtility.CalculateBaseDamage(calculateInfo.BaseAttack + calculateInfo.IncreaseAttack,
            calculateInfo.BaseRestraint + calculateInfo.IncreaseRestraint,
            calculateInfo.BaseElementAttack + calculateInfo.IncreaseElementAttack,
            calculateInfo.BaseBreakDefense + calculateInfo.IncreaseBreakDefense, enemy.Defense, enemy.AntiRestraint,
            rate1, rate2, calculateInfo.BaseBreakAirShield + calculateInfo.IncreaseBreakAirShield, enemy.AirShield,
            false);

        return Math.Round(damage,1);
    }
}