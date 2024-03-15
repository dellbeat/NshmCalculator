using NshmCalculator.Shared.Models.BaseModel;
using NshmCalculator.Shared.Models.CalculatorModel;
using NshmCalculator.Shared.Models.CalculatorModel.KI;
using NshmCalculator.Shared.Models.PageModel;

namespace NshmCalculator.Test.CalculatorUtility;

public partial class CalculatorUtilityTest
{
    /// <summary>
    /// 系数1
    /// </summary>
    private const double Rate1 = 0.324 * 923;

    /// <summary>
    /// 系数2，暂定为固定值
    /// </summary>
    private const double Rate2 = 0.324;

    /// <summary>
    /// 误差限制
    /// </summary>
    private const double DeviationLimit = 0.001;

    /// <summary>
    /// 投放数据
    /// </summary>
    private readonly DeliveryData _deliveryData = new();

    #region 属性收益计算器相关

    /// <summary>
    /// 用于验证各类情况下计算准确性的测试数据，均已使用原EXCEL进行对比
    /// </summary>
    private static IEnumerable<AttributePageModel> ResultTestData
    {
        get
        {
            yield return new AttributePageModel
            {
                PageCalculateInfo = new AttributeCalculateInfo
                {
                    BaseAttack = 2500,
                    BaseRestraint = 1000,
                    BaseElementAttack = 1000,
                    BaseBreakDefense = 1000,
                    BaseHit = 500,
                    BaseCriticalHits = 888,
                    BaseCriticalRate = 188,
                    BaseZtCriticalHitsRate = 5,
                    BaseRestrainedRate = 10,
                    BaseBreakAirShield = 206,
                    NonCriticalDamageFunValue = 1659.4,
                    CriticalDamageFunValue = 2268.6
                },
                PageEnemyInfo = new EnemyInfo
                    { Block = 590, Defense = 1600, AntiElementAttack = 0, AntiCriticalHits = 200, AirShield = 206 },
                DamageInfoHistory = []
            };
            yield return new AttributePageModel
            {
                PageCalculateInfo = new AttributeCalculateInfo
                {
                    BaseAttack = 2500,
                    BaseRestraint = 1000,
                    BaseElementAttack = 1000,
                    BaseBreakDefense = 1000,
                    BaseHit = 500,
                    BaseCriticalHits = 888,
                    BaseCriticalRate = 188,
                    BaseZtCriticalHitsRate = 5,
                    BaseRestrainedRate = 10,
                    NonCriticalDamageFunValue = 1659.4,
                    CriticalDamageFunValue = 2268.6
                },
                PageEnemyInfo = new EnemyInfo
                    { Block = 590, Defense = 1600, AntiElementAttack = 0, AntiCriticalHits = 200 },
                DamageInfoHistory = []
            };
            yield return new AttributePageModel
            {
                PageCalculateInfo = new AttributeCalculateInfo
                {
                    BaseAttack = 2200,
                    BaseRestraint = 1200,
                    BaseElementAttack = 850,
                    BaseBreakDefense = 950,
                    BaseHit = 400,
                    BaseCriticalHits = 700,
                    BaseCriticalRate = 150,
                    BaseZtCriticalHitsRate = 4,
                    BaseRestrainedRate = 15,
                    NonCriticalDamageFunValue = 1497.2,
                    CriticalDamageFunValue = 1663.5
                },
                PageEnemyInfo = new EnemyInfo
                {
                    Block = 550, Defense = 1400, AntiElementAttack = 100, AntiCriticalHits = 175, AntiRestraint = 500
                },
                DamageInfoHistory = []
            };

            yield return new AttributePageModel
            {
                PageCalculateInfo = new AttributeCalculateInfo
                {
                    BaseAttack = 3000,
                    BaseRestraint = 800,
                    BaseElementAttack = 1150,
                    BaseBreakDefense = 1100,
                    BaseHit = 600,
                    BaseCriticalHits = 950,
                    BaseCriticalRate = 200,
                    BaseZtCriticalHitsRate = 3,
                    BaseRestrainedRate = 12,
                    NonCriticalDamageFunValue = 1539.7,
                    CriticalDamageFunValue = 2269.0
                },
                PageEnemyInfo = new EnemyInfo
                {
                    Block = 610, Defense = 1700, AntiElementAttack = 200, AntiCriticalHits = 225, AntiRestraint = 600
                },
                DamageInfoHistory = []
            };
            yield return new AttributePageModel
            {
                PageCalculateInfo = new AttributeCalculateInfo
                {
                    BaseAttack = 2750,
                    BaseRestraint = 900,
                    BaseElementAttack = 950,
                    BaseBreakDefense = 1050,
                    BaseHit = 450,
                    BaseCriticalHits = 800,
                    BaseCriticalRate = 175,
                    BaseZtCriticalHitsRate = 3.5,
                    BaseRestrainedRate = 14,
                    NonCriticalDamageFunValue = 1509.5,
                    CriticalDamageFunValue = 1865.9
                },
                PageEnemyInfo = new EnemyInfo
                {
                    Block = 580, Defense = 1500, AntiElementAttack = 150, AntiCriticalHits = 225, AntiRestraint = 700
                },
                DamageInfoHistory = []
            };
            yield return new AttributePageModel //命中超满命中、破防超满防御的情况
            {
                PageCalculateInfo = new AttributeCalculateInfo
                {
                    BaseAttack = 2750,
                    BaseRestraint = 900,
                    BaseElementAttack = 950,
                    BaseBreakDefense = 1600,
                    BaseHit = 800,
                    BaseCriticalHits = 800,
                    BaseCriticalRate = 175,
                    BaseZtCriticalHitsRate = 3.5,
                    BaseRestrainedRate = 14,
                    NonCriticalDamageFunValue = 1704.0,
                    CriticalDamageFunValue = 2307.3
                },
                PageEnemyInfo = new EnemyInfo
                {
                    Block = 580, Defense = 1500, AntiElementAttack = 150, AntiCriticalHits = 225, AntiRestraint = 700
                },
                DamageInfoHistory = []
            };
            yield return new AttributePageModel
            {
                PageCalculateInfo = new AttributeCalculateInfo //气盾/破盾等不为0时的情况
                {
                    BaseAttack = 2750,
                    BaseRestraint = 1000,
                    BaseElementAttack = 950,
                    BaseBreakDefense = 1050,
                    BaseHit = 450,
                    BaseCriticalHits = 800,
                    BaseCriticalRate = 175,
                    BaseZtCriticalHitsRate = 3.5,
                    BaseRestrainedRate = 14,
                    BaseBreakAirShield = 100,
                    NonCriticalDamageFunValue = 1509.5,
                    CriticalDamageFunValue = 1865.9,
                },
                PageEnemyInfo = new EnemyInfo
                {
                    Block = 580, Defense = 1500, AntiElementAttack = 150, AntiCriticalHits = 225, AntiRestraint = 700,
                    AirShield = 300
                },
                DamageInfoHistory = []
            };
        }
    }

    #endregion

    #region 增伤率计算器相关

    /// <summary>
    /// 增伤率计算器测试数据
    /// </summary>
    private static IEnumerable<DamageRatePageModel> DamageRateTestData
    {
        get
        {
            yield return new DamageRatePageModel
            {
                PageCalculateInfo = new()
                {
                    BaseAttack = 2500,
                    BaseRestraint = 1000,
                    BaseElementAttack = 1000,
                    BaseBreakDefense = 1000,
                    BaseHit = 500,
                    BaseCriticalHits = 888,
                    BaseCriticalRate = 188,
                    BaseZtCriticalHitsRate = 1,
                    IncreaseAttack = 100,
                    IncreaseBreakDefense = 100,
                    IncreaseElementAttack = 100,
                    IncreaseRestraint = 100,
                    IncreaseHit = 100,
                    IncreaseCriticalHits = 100,
                    IncreaseCriticalRate = 1,
                    Result = 0.1553
                },
                PageEnemyInfo = new()
                {
                    Block = 630,
                    Defense = 1680,
                    AntiElementAttack = 0
                }
            };
            yield return new DamageRatePageModel
            {
                PageCalculateInfo = new() //命中超出的情况
                {
                    BaseAttack = 3500,
                    BaseRestraint = 1500,
                    BaseElementAttack = 1500,
                    BaseBreakDefense = 1500,
                    BaseHit = 821,
                    BaseCriticalHits = 1000,
                    BaseCriticalRate = 200,
                    BaseZtCriticalHitsRate = 2,
                    IncreaseAttack = 300,
                    IncreaseBreakDefense = 300,
                    IncreaseElementAttack = 300,
                    IncreaseRestraint = 300,
                    IncreaseHit = 200,
                    IncreaseCriticalHits = 300,
                    IncreaseCriticalRate = 2,
                    Result = 0.3141
                },
                PageEnemyInfo = new()
                {
                    Block = 800,
                    Defense = 2000,
                    AntiElementAttack = 100
                }
            };
            yield return new DamageRatePageModel
            {
                PageCalculateInfo = new() //超破防的情况
                {
                    BaseAttack = 4000,
                    BaseRestraint = 2000,
                    BaseElementAttack = 2000,
                    BaseBreakDefense = 2000,
                    BaseHit = 1000,
                    BaseCriticalHits = 1200,
                    BaseCriticalRate = 220,
                    BaseZtCriticalHitsRate = 3,
                    IncreaseAttack = 500,
                    IncreaseBreakDefense = 500,
                    IncreaseElementAttack = 500,
                    IncreaseRestraint = 500,
                    IncreaseHit = 500,
                    IncreaseCriticalHits = 500,
                    IncreaseCriticalRate = 3,
                    Result = 0.3780
                },
                PageEnemyInfo = new()
                {
                    Block = 900,
                    Defense = 2300,
                    AntiElementAttack = 200
                }
            };
            yield return new DamageRatePageModel
            {
                PageCalculateInfo = new()
                {
                    BaseAttack = 4500,
                    BaseRestraint = 2500,
                    BaseElementAttack = 2500,
                    BaseBreakDefense = 2500,
                    BaseHit = 1250,
                    BaseCriticalHits = 1500,
                    BaseCriticalRate = 240,
                    BaseZtCriticalHitsRate = 4,
                    IncreaseAttack = 700,
                    IncreaseBreakDefense = 700,
                    IncreaseElementAttack = 700,
                    IncreaseRestraint = 700,
                    IncreaseHit = 700,
                    IncreaseCriticalHits = 700,
                    IncreaseCriticalRate = 4,
                    Result = 0.3443
                },
                PageEnemyInfo = new()
                {
                    Block = 1000,
                    Defense = 2600,
                    AntiElementAttack = 300
                }
            };
        }
    }

    #endregion

    private static IEnumerable<object> PerGainsTestData
    {
        get
        {
            yield return new object[]
            {
                new PlayerBaseInfo
                {
                    BaseAttack = 4000,
                    BaseRestraint = 2800,
                    BaseElementAttack = 1200,
                    BaseBreakDefense = 1800,
                    BaseHit = 850,
                    BaseCriticalHits = 1300,
                    BaseCriticalRate = 189,
                    BaseZtCriticalHitsRate = 5
                },
                new EnemyInfo
                {
                    Block = 820,
                    Defense = 3950,
                    AntiElementAttack = 60,
                    AntiRestraint = 1500,
                    AntiCriticalHits = 500,
                },
                new KiEarningRate
                {
                    AttackAndRestraint = 0.0001216990,
                    ElementAttack = 0.0002131860,
                    BreakDefense = 0.0001547043,
                    Hit = 0.0002807138,
                    CriticalHits = 0.0002021011,
                    CriticalRate = 0.0037739486,
                    Strength = 0.0009179036,
                    Lightness = 0.0005479327
                }
            };
            yield return new object[]
            {
                new PlayerBaseInfo
                {
                    BaseAttack = 4500,
                    BaseRestraint = 3100,
                    BaseElementAttack = 1500,
                    BaseBreakDefense = 2000,
                    BaseHit = 900,
                    BaseCriticalHits = 1600,
                    BaseCriticalRate = 200,
                    BaseZtCriticalHitsRate = 5
                },
                new EnemyInfo
                {
                    Block = 850,
                    Defense = 4200,
                    AntiElementAttack = 60,
                    AntiRestraint = 1700,
                    AntiCriticalHits = 600,
                },
                new KiEarningRate
                {
                    AttackAndRestraint = 0.0001067499,
                    ElementAttack = 0.0001888652,
                    BreakDefense = 0.0001472859,
                    Hit = 0.0002726636,
                    CriticalHits = 0.0001694485,
                    CriticalRate = 0.0038782695,
                    Strength = 0.0008283212,
                    Lightness = 0.0004892484
                }
            };

            yield return new object[]
            {
                new PlayerBaseInfo
                {
                    BaseAttack = 5200,
                    BaseRestraint = 3200,
                    BaseElementAttack = 1600,
                    BaseBreakDefense = 2500,
                    BaseHit = 950,
                    BaseCriticalHits = 1410,
                    BaseCriticalRate = 189.9,
                    BaseZtCriticalHitsRate = 5
                },
                new EnemyInfo
                {
                    Block = 915,
                    Defense = 3980,
                    AntiElementAttack = 60,
                    AntiRestraint = 1480,
                    AntiCriticalHits = 530,
                },
                new KiEarningRate
                {
                    AttackAndRestraint = 0.0000982615,
                    ElementAttack = 0.0001491101,
                    BreakDefense = 0.0001824225,
                    Hit = 0.0002506283,
                    CriticalHits = 0.0001832570,
                    CriticalRate = 0.0038771530,
                    Strength = 0.0008561527,
                    Lightness = 0.0004937138
                }
            };

            yield return new object[]
            {
                new PlayerBaseInfo
                {
                    BaseAttack = 3750,
                    BaseRestraint = 3150,
                    BaseElementAttack = 1100,
                    BaseBreakDefense = 1890,
                    BaseHit = 790,
                    BaseCriticalHits = 1150,
                    BaseCriticalRate = 235,
                    BaseZtCriticalHitsRate = 5
                },
                new EnemyInfo
                {
                    Block = 865,
                    Defense = 3890,
                    AntiElementAttack = 60,
                    AntiRestraint = 1600,
                    AntiCriticalHits = 490,
                },
                new KiEarningRate
                {
                    AttackAndRestraint = 0.0001251992,
                    ElementAttack = 0.0002127511,
                    BreakDefense = 0.0001641870,
                    Hit = 0.0003314912,
                    CriticalHits = 0.0003167163,
                    CriticalRate = 0.0029937042,
                    Strength = 0.0009543699,
                    Lightness = 0.0007719391
                }
            };
        }
    }
}