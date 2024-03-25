using NshmCalculator.Shared.Models.BaseModel;
using NshmCalculator.Shared.Models.BaseModel.Enums;
using NshmCalculator.Shared.Models.CalculatorModel;
using NshmCalculator.Shared.Models.CalculatorModel.KI;
using NshmCalculator.Shared.Models.PageModel;

namespace NshmCalculator.Test.CalculatorUtility;

public partial class CalculatorUtilityTest
{
    [SetUp]
    public void Setup()
    {
    }

    #region 气盾特性相关

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

        double damage1 = Shared.CalculatorUtility.CalculateBaseDamage(
            calculateInfo.BaseAttack + calculateInfo.IncreaseAttack,
            calculateInfo.BaseRestraint + calculateInfo.IncreaseRestraint,
            calculateInfo.BaseElementAttack + calculateInfo.IncreaseElementAttack,
            calculateInfo.BaseBreakDefense + calculateInfo.IncreaseBreakDefense, enemy.Defense, enemy.AntiRestraint,
            Rate1, Rate2, calculateInfo.BaseBreakAirShield + calculateInfo.IncreaseBreakAirShield, enemy.AirShield,
            true);

        double damage2 = Shared.CalculatorUtility.CalculateBaseDamage(
            calculateInfo.BaseAttack + calculateInfo.IncreaseAttack,
            calculateInfo.BaseRestraint + calculateInfo.IncreaseRestraint,
            calculateInfo.BaseElementAttack + calculateInfo.IncreaseElementAttack,
            calculateInfo.BaseBreakDefense + calculateInfo.IncreaseBreakDefense, enemy.Defense, enemy.AntiRestraint,
            Rate1, Rate2, calculateInfo.BaseBreakAirShield + calculateInfo.IncreaseBreakAirShield, enemy.AirShield);

        Assert.True(Math.Abs(damage1 - damage2) < DeviationLimit);

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

        double damage1 = Shared.CalculatorUtility.CalculateBaseDamage(
            calculateInfo.BaseAttack + calculateInfo.IncreaseAttack,
            calculateInfo.BaseRestraint + calculateInfo.IncreaseRestraint,
            calculateInfo.BaseElementAttack + calculateInfo.IncreaseElementAttack,
            calculateInfo.BaseBreakDefense + calculateInfo.IncreaseBreakDefense, enemy.Defense, enemy.AntiRestraint,
            Rate1, Rate2, calculateInfo.BaseBreakAirShield + calculateInfo.IncreaseBreakAirShield, enemy.AirShield,
            true);

        calculateInfo.IncreaseRestraint = 0; //降低剩余气盾，同时降低与之相对的克制数值

        double damage2 = Shared.CalculatorUtility.CalculateBaseDamage(
            calculateInfo.BaseAttack + calculateInfo.IncreaseAttack,
            calculateInfo.BaseRestraint + calculateInfo.IncreaseRestraint,
            calculateInfo.BaseElementAttack + calculateInfo.IncreaseElementAttack,
            calculateInfo.BaseBreakDefense + calculateInfo.IncreaseBreakDefense, enemy.Defense, enemy.AntiRestraint,
            Rate1, Rate2, calculateInfo.BaseBreakAirShield + calculateInfo.IncreaseBreakAirShield, enemy.AirShield);

        Assert.True(Math.Abs(damage1 - damage2) < DeviationLimit);

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

        double damage1 = Shared.CalculatorUtility.CalculateBaseDamage(
            calculateInfo.BaseAttack + calculateInfo.IncreaseAttack,
            calculateInfo.BaseRestraint + calculateInfo.IncreaseRestraint,
            calculateInfo.BaseElementAttack + calculateInfo.IncreaseElementAttack,
            calculateInfo.BaseBreakDefense + calculateInfo.IncreaseBreakDefense, enemy.Defense, enemy.AntiRestraint,
            Rate1, Rate2, calculateInfo.BaseBreakAirShield + calculateInfo.IncreaseBreakAirShield, enemy.AirShield,
            true);

        calculateInfo.IncreaseRestraint = 0; //降低剩余气盾，同时降低与之相对的克制数值

        double damage2 = Shared.CalculatorUtility.CalculateBaseDamage(
            calculateInfo.BaseAttack + calculateInfo.IncreaseAttack,
            calculateInfo.BaseRestraint + calculateInfo.IncreaseRestraint,
            calculateInfo.BaseElementAttack + calculateInfo.IncreaseElementAttack,
            calculateInfo.BaseBreakDefense + calculateInfo.IncreaseBreakDefense, enemy.Defense, enemy.AntiRestraint,
            Rate1, Rate2, calculateInfo.BaseBreakAirShield + calculateInfo.IncreaseBreakAirShield, enemy.AirShield);

        Assert.True(Math.Abs(damage1 - damage2) < DeviationLimit);

        Assert.Pass("玩家破盾大于等于敌方1/3气盾值且小于气盾值-测试通过");
    }

    #endregion

    #region 属性收益计算器相关

    /// <summary>
    /// 属性收益计算器测试方法(1.3之前）
    /// </summary>
    /// <param name="model"></param>
    [Test, TestCaseSource(nameof(ResultTestData))]
    public void ResultTest(AttributePageModel model)
    {
        var calculateInfo = model.PageCalculateInfo;
        var enemyInfo = model.PageEnemyInfo;

        calculateInfo.BaseDamageFunValue = Shared.CalculatorUtility.CalculateBaseDamage(
            calculateInfo.BaseAttack + calculateInfo.IncreaseAttack,
            calculateInfo.BaseRestraint + calculateInfo.IncreaseRestraint,
            calculateInfo.BaseElementAttack + calculateInfo.IncreaseElementAttack,
            calculateInfo.BaseBreakDefense + calculateInfo.IncreaseBreakDefense, enemyInfo.Defense,
            enemyInfo.AntiRestraint, Rate1, Rate2,
            calculateInfo.BaseBreakAirShield + calculateInfo.IncreaseBreakAirShield, enemyInfo.AirShield, true,
            enemyInfo.AntiElementAttack);
        var nonCriticalDamageFunValue = Shared.CalculatorUtility.CalculateNonCriticalDamage(
            calculateInfo.BaseDamageFunValue,
            (calculateInfo.BaseRestrainedRate + calculateInfo.IncreaseRestrainedRate) / 100.0);
        var criticalDamageFunValue = Shared.CalculatorUtility.CalculateCriticalDamage(nonCriticalDamageFunValue,
            calculateInfo.BaseHit + calculateInfo.IncreaseHit, enemyInfo.Block,
            (calculateInfo.BaseCriticalRate + calculateInfo.IncreaseCriticalRate - 100) / 100.0,
            calculateInfo.BaseCriticalHits + calculateInfo.IncreaseCriticalHits, enemyInfo.AntiCriticalHits,
            calculateInfo.BaseZtCriticalHitsRate / 100.0, out _);

        Assert.True(Math.Abs(Math.Round(nonCriticalDamageFunValue, 1) - calculateInfo.NonCriticalDamageFunValue) <
                    DeviationLimit);
        Assert.True(Math.Abs(Math.Round(criticalDamageFunValue, 1) - calculateInfo.CriticalDamageFunValue) <
                    DeviationLimit);

        Assert.Pass("未会心/会心命中期望数据-测试通过");
    }

    #endregion

    #region 增伤率计算器相关

    /// <summary>
    /// 增伤计算器测试方法(1.1命中算法)
    /// </summary>
    /// <param name="model">需测试的数据</param>
    [Test, TestCaseSource(nameof(DamageRateTestData))]
    public void DamageRateTest(DamageRatePageModel model)
    {
        var baseInfo = model.PageCalculateInfo;
        var enemyInfo = model.PageEnemyInfo;

        var percent = Shared.CalculatorUtility.CalculateIncreaseRate(baseInfo, enemyInfo, baseInfo.IncreaseAttack,
            baseInfo.IncreaseBreakDefense, baseInfo.IncreaseElementAttack, baseInfo.IncreaseRestraint,
            baseInfo.IncreaseHit, baseInfo.IncreaseCriticalHits, baseInfo.IncreaseCriticalRate, true,
            HitCalculateVersion.Version11);

        Assert.True(Math.Abs(percent.Item1 - baseInfo.Result) < DeviationLimit);

        Assert.Pass("增伤率数值-测试通过");
    }

    #endregion

    [Test, TestCaseSource(nameof(PerGainsTestData))]
    public void PerGainsTest(PlayerBaseInfo baseInfo, EnemyInfo enemyInfo, KiEarningRate rate)
    {
        var calRate = Shared.CalculatorUtility.CalculatePerGains(baseInfo, enemyInfo, _deliveryData);

        Assert.True(Math.Abs(calRate.AttackAndRestraint - rate.AttackAndRestraint) < DeviationLimit);
        Assert.True(Math.Abs(calRate.ElementAttack - rate.ElementAttack) < DeviationLimit);
        Assert.True(Math.Abs(calRate.BreakDefense - rate.BreakDefense) < DeviationLimit);
        Assert.True(Math.Abs(calRate.Hit - rate.Hit) < DeviationLimit);
        Assert.True(Math.Abs(calRate.CriticalHits - rate.CriticalHits) < DeviationLimit);
        Assert.True(Math.Abs(calRate.CriticalRate - rate.CriticalRate) < DeviationLimit);
        Assert.True(Math.Abs(calRate.Strength - rate.Strength) < DeviationLimit);
        Assert.True(Math.Abs(calRate.Lightness - rate.Lightness) < DeviationLimit);

        Assert.Pass("提升收益-测试通过");
    }
}