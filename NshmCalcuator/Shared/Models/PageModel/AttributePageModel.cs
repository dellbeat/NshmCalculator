using NshmCalculator.Shared.Models.BaseModel;
using NshmCalculator.Shared.Models.CalculatorModel;
using System.ComponentModel.DataAnnotations;

namespace NshmCalculator.Shared.Models.PageModel;

public class AttributePageModel
{
    [ValidateComplexType]
    public AttributeCalculateInfo PageCalculateInfo { get; set; } = new()
    {
        BaseAttack = 10000,
        BaseRestraint = 11000,
        BaseElementAttack = 2500,
        BaseBreakDefense = 5000,
        BaseHit = 2700,
        BaseCriticalHits = 4200,
        BaseCriticalRate = 165,
        BaseRestrainedRate = 42,
        BaseBreakAirShield = 800,
        BaseIgnoreAntiElement = 1300,
        BaseZtCriticalHitsRate = 5
    };

    [ValidateComplexType]
    public EnemyInfo PageEnemyInfo { get; set; }

    /// <summary>
    /// 最近一次使用的首领名称
    /// </summary>
    public string LastEnemy { get; set; }
}