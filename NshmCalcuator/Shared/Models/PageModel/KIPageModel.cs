using NshmCalculator.Shared.Models.BaseModel;
using NshmCalculator.Shared.Models.CalculatorModel;
using System.ComponentModel.DataAnnotations;

namespace NshmCalculator.Shared.Models.PageModel;

public class KIPageModel
{
    [ValidateComplexType]
    public KICalculateInfo PageCalculateInfo { get; set; } = new()
    {
        BaseAttack = 4000,
        BaseRestraint = 2800,
        BaseElementAttack = 1200,
        BaseBreakDefense = 1800,
        BaseHit = 850,
        BaseCriticalHits = 1300,
        BaseCriticalRate = 189,
        BaseZtCriticalHitsRate = 5
    };

    [ValidateComplexType]
    public EnemyInfo PageEnemyInfo { get; set; } = new()
    {
        Block = 820,
        Defense = 3950,
        AntiRestraint = 1500,
        AntiCriticalHits = 500,
        AntiElementAttack = 60
    };

    [ValidateComplexType]
    public List<KIInfo> KIInfos { get; set; } = new();

    public List<AttributeCalculateInfo> DamageInfoHistory = new();
    
    /// <summary>
    /// 1.3命中
    /// </summary>
    public bool Version13Mode { get; set; } = false;
}