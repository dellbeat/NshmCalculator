namespace NshmCalculator.Shared;

public class PlayerInfo
{
    #region 玩家基础属性

    /// <summary>
    /// 玩家基础攻击力
    /// </summary>
    public int PlayerBaseAttack { get; set; }

    /// <summary>
    /// 玩家基础克制
    /// </summary>
    public int PlayerBaseRestraint { get; set; }
    
    /// <summary>
    /// 玩家基础属性（元素）攻击
    /// </summary>
    public int PlayerBaseElementAttack { get; set; }
    
    /// <summary>
    /// 玩家基础破防
    /// </summary>
    public int PlayerBaseBreakDefense { get; set; }
    
    /// <summary>
    /// 玩家基础命中
    /// </summary>
    public int PlayerBaseHit { get; set; }
    
    /// <summary>
    /// 玩家基础会心
    /// </summary>
    public int PlayerBaseCriticalHits { get; set; }
    
    /// <summary>
    /// 玩家基础会伤率
    /// </summary>
    public double PlayerBaseCriticalRate { get; set; }
    
    /// <summary>
    /// 玩家周天会心率
    /// </summary>
    public double PlayerBaseZtCriticalHitsRate { get; set; }

    #endregion

    #region 新增数值

    /// <summary>
    /// 新增攻击力
    /// </summary>
    public int IncreaseAttack { get; set; }
    
    /// <summary>
    /// 新增克制
    /// </summary>
    public int IncreaseBaseRestraint { get; set; }
    
    /// <summary>
    /// 新增属性攻击
    /// </summary>
    public int IncreaseElementAttack { get; set; }
    
    /// <summary>
    /// 新增破防
    /// </summary>
    public int IncreaseBreakDefense { get; set; }
    
    /// <summary>
    /// 新增命中
    /// </summary>
    public int IncreaseHit { get; set; }
    
    /// <summary>
    /// 新增会心
    /// </summary>
    public int IncreaseCriticalHits { get; set; }
    
    /// <summary>
    /// 新增会伤率
    /// </summary>
    public double IncreaseCriticalRate { get; set; }

    #endregion

    #region 敌方数值

    /// <summary>
    /// 敌方防御
    /// </summary>
    public double EnemyDefense { get; set; }
    
    /// <summary>
    /// 敌方格挡
    /// </summary>
    public int EnemyBlock { get; set; }
    
    /// <summary>
    /// 满命中
    /// </summary>
    public int FullHit { get; set; }

    #endregion
}