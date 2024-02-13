using System.ComponentModel.DataAnnotations;

namespace NshmCalculator.Shared.Models;

public class QuickPlayerInfo : PlayerBaseInfo
{
    #region 新增数值

    /// <summary>
    /// 新增攻击力
    /// </summary>
    [Required]
    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseAttack { get; set; }

    /// <summary>
    /// 新增克制
    /// </summary>
    [Required]
    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseRestraint { get; set; }

    /// <summary>
    /// 新增属性攻击
    /// </summary>
    [Required]
    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseElementAttack { get; set; }

    /// <summary>
    /// 新增破防
    /// </summary>
    [Required]
    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseBreakDefense { get; set; }

    /// <summary>
    /// 新增命中
    /// </summary>
    [Required]
    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseHit { get; set; }

    /// <summary>
    /// 新增会心
    /// </summary>
    [Required]
    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseCriticalHits { get; set; }

    /// <summary>
    /// 新增会伤率
    /// </summary>
    [Required]
    [Range(0, 10, ErrorMessage = "请保证新增会伤百分比在0.0-10.0范围内")]
    public double IncreaseCriticalRate { get; set; }

    #endregion

    public DateTime? LastCalTime { get; set; }

    public QuickPlayerInfo()
    {
        PlayerBaseAttack = 2500;
        PlayerBaseRestraint = 1000;
        PlayerBaseElementAttack = 1000;
        PlayerBaseBreakDefense = 1000;
        PlayerBaseHit = 500;
        PlayerBaseCriticalHits = 888;
        PlayerBaseCriticalRate = 188;
        EnemyBlock = 630;
        EnemyDefense = 1680;
    }
}