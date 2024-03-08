using NshmCalculator.Shared.Models.BaseModel;
using System.ComponentModel.DataAnnotations;

namespace NshmCalculator.Shared.Models.CalculatorModel;

/// <summary>
/// 内功收益计算器计算数值
/// </summary>
public class KICalculateInfo : PlayerBaseInfo
{
    //TODO:后续需要对存储结构进行重构

    #region 新增数值

    /// <summary>
    /// 新增耐力
    /// </summary>
    [Required]
    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseStamina { get; set; }

    /// <summary>
    /// 新增根骨
    /// </summary>
    [Required]
    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseVitality { get; set; }

    /// <summary>
    /// 新增气海/力量
    /// </summary>
    [Required]
    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseStrength { get; set; }

    /// <summary>
    /// 新增身法（轻功）
    /// </summary>
    [Required]
    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseLightness { get; set; }

    /// <summary>
    /// 新增攻击力
    /// </summary>
    [Required]
    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseFullAttack { get; set; }

    /// <summary>
    /// 新增大小攻
    /// </summary>
    [Required]
    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseHalfAttack { get; set; }

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

    /// <summary>
    /// 攻击分
    /// </summary>
    public double Score { get; set; }

    public KICalculateInfo()
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