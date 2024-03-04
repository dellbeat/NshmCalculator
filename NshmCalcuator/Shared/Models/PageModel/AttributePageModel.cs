using NshmCalculator.Shared.Models.BaseModel;
using NshmCalculator.Shared.Models.CalculatorModel;

namespace NshmCalculator.Shared.Models.PageModel;

public class AttributePageModel
{
    public AttributePlayerInfo PageCalculateInfo { get; set; } = new AttributePlayerInfo()
    {
        BaseAttack = 2500,
        BaseRestraint = 1000,
        BaseElementAttack = 1000,
        BaseBreakDefense = 1000,
        BaseHit = 500,
        BaseCriticalHits = 888,
        BaseCriticalRate = 188,
    };

    public EnemyInfo PageEnemyInfo { get; set; } = new()
    {
        Block = 630,
        Defense = 1680
    };

    public List<AttributePlayerInfo> DamageInfoHistory = new();
}