using System.ComponentModel.DataAnnotations;
using NshmCalculator.Shared.Models.BaseModel;

namespace NshmCalculator.Shared.Models.CalculatorModel;

public class AttributePlayerInfo : PlayerBaseInfo
{
    #region 玩家基础属性

    /// <summary>
    /// 克制百分比
    /// </summary>
    [Required]
    [Range(0, 100, ErrorMessage = "请保证克制百分比在0.0-100.0范围内")]
    public double BaseRestrainedRate { get; set; }

    #endregion

    #region 新增数值

    /// <summary>
    /// 新增攻击力
    /// </summary>
    [Required]
    [Range(-100000, 100000, ErrorMessage = "请输入-100000至100000内的整数")]
    public int IncreaseAttack { get; set; }

    /// <summary>
    /// 新增破防
    /// </summary>
    [Required]
    [Range(-100000, 100000, ErrorMessage = "请输入-100000至100000内的整数")]
    public int IncreaseBreakDefense { get; set; }

    /// <summary>
    /// 新增元素攻击
    /// </summary>
    [Required]
    [Range(-100000, 100000, ErrorMessage = "请输入-100000至100000内的整数")]
    public int IncreaseElementAttack { get; set; }

    /// <summary>
    /// 新增克制
    /// </summary>
    [Required]
    [Range(-100000, 100000, ErrorMessage = "请输入-100000至100000内的整数")]
    public int IncreaseRestraint { get; set; }

    /// <summary>
    /// 新增命中
    /// </summary>
    [Required]
    [Range(-100000, 100000, ErrorMessage = "请输入-100000至100000内的整数")]
    public int IncreaseHit { get; set; }

    /// <summary>
    /// 新增会心
    /// </summary>
    [Required]
    [Range(-100000, 100000, ErrorMessage = "请输入-100000至100000内的整数")]
    public int IncreaseCriticalHits { get; set; }

    /// <summary>
    /// 新增会伤率
    /// </summary>
    [Required]
    [Range(0, 10, ErrorMessage = "请保证新增会伤百分比在0.0-10.0范围内")]
    public double IncreaseCriticalRate { get; set; }

    /// <summary>
    /// 新增会伤百分比
    /// </summary>
    [Required]
    [Range(0, 5, ErrorMessage = "请保证新增会心百分比在0.0-5.0范围内")]
    public double IncreaseCriticalHitsRate { get; set; }

    /// <summary>
    /// 新增克制百分比
    /// </summary>
    [Required]
    [Range(0, 100, ErrorMessage = "请保证新增克制百分比在0.0-100.0范围内")]
    public double IncreaseRestrainedRate { get; set; }


    #endregion

    #region 计算结果

    /// <summary>
    /// 无首领加成的基础伤害
    /// </summary>
    public double BaseDamageFunValue { get; set; }

    /// <summary>
    /// 未会心伤害
    /// </summary>
    public double NonCriticalDamageFunValue { get; set; }

    /// <summary>
    /// 包含命中的伤害期望
    /// </summary>
    public double CriticalDamageFunValue { get; set; }

    /// <summary>
    /// 实际会心率
    /// </summary>
    public double CalCriticalRate { get; set; }

    /// <summary>
    /// 伤害期望提升百分比
    /// </summary>
    public double ImprovePercent { get; set; }

    /// <summary>
    /// 上次计算时间
    /// </summary>
    public DateTime? LastCalTime { get; set; }

    #endregion

    public AttributePlayerInfo()
    {
        BaseAttack = 2500;
        BaseRestraint = 1000;
        BaseElementAttack = 1000;
        BaseBreakDefense = 1000;
        BaseHit = 500;
        BaseCriticalHits = 888;
        BaseCriticalRate = 188;
    }
}