using NshmCalculator.Shared.Models.BaseModel;
using NshmCalculator.Shared.Models.CalculatorModel;
using System.ComponentModel.DataAnnotations;

namespace NshmCalculator.Shared.Models.PageModel;

public class DamageRatePageModel
{
    [ValidateComplexType]
    public DamageRateCalculateInfo PageCalculateInfo { get; set; } = new()
    {
        BaseAttack = 2500,
        BaseRestraint = 1000,
        BaseElementAttack = 1000,
        BaseBreakDefense = 1000,
        BaseHit = 500,
        BaseCriticalHits = 888,
        BaseCriticalRate = 188.0
    };

    [ValidateComplexType]
    public EnemyInfo PageEnemyInfo { get; set; } = new()
    {
        Block = 630,
        Defense = 1680
    };

    public List<DamageRateCalculateInfo> DamageInfoHistory { get; set; } = new();
}