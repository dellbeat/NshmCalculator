namespace NshmCalculator.Shared.Models.Interface;

/// <summary>
/// 供内功计算器计算详情和内功信息使用，表示为新增/内功自身关联属性
/// </summary>
public interface ICharacterAttributes
{
    /// <summary>
    /// 新增耐力
    /// </summary>
    int IncreaseStamina { get; set; }

    /// <summary>
    /// 新增根骨
    /// </summary>
    int IncreaseVitality { get; set; }

    /// <summary>
    /// 新增气海/力量
    /// </summary>
    int IncreaseStrength { get; set; }

    /// <summary>
    /// 新增身法（轻功）
    /// </summary>
    int IncreaseLightness { get; set; }

    /// <summary>
    /// 新增攻击力
    /// </summary>
    int IncreaseFullAttack { get; set; }

    /// <summary>
    /// 新增大小攻
    /// </summary>
    int IncreaseHalfAttack { get; set; }

    /// <summary>
    /// 新增属性攻击
    /// </summary>
    int IncreaseElementAttack { get; set; }

    /// <summary>
    /// 新增克制
    /// </summary>
    int IncreaseRestraint { get; set; }

    /// <summary>
    /// 新增会心
    /// </summary>
    int IncreaseCriticalHits { get; set; }

    /// <summary>
    /// 新增破防
    /// </summary>
    int IncreaseBreakDefense { get; set; }

    /// <summary>
    /// 新增命中
    /// </summary>
    int IncreaseHit { get; set; }
}