using System.ComponentModel.DataAnnotations;

namespace NshmCalculator.Shared.Models.BaseModel;

/// <summary>
/// 敌方基础数值
/// </summary>
public class EnemyInfo : ICloneable
{
    /// <summary>
    /// 敌方防御
    /// </summary>
    [Required]
    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int Defense { get; set; }

    /// <summary>
    /// 敌方格挡
    /// </summary>
    [Required]
    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int Block { get; set; }

    /// <summary>
    /// 敌方抵御
    /// </summary>
    [Required]
    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int AntiRestraint { get; set; }

    /// <summary>
    /// 敌方会心抗
    /// </summary>
    [Required]
    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int AntiCriticalHits { get; set; }

    /// <summary>
    /// 敌方元素抗
    /// </summary>
    [Required]
    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int AntiElementAttack { get; set; }

    /// <summary>
    /// 敌方气盾
    /// </summary>
    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int AirShield { get; set; }

    #region 系数

    /// <summary>
    /// 满命中所需系数，1.3之后每个木桩该系数都会有变化
    /// </summary>
    public int FullHitCoe { get; set; } = 6045;

    /// <summary>
    /// 会心率所需系数1
    /// </summary>
    public int CriticalHitLeftCoe { get; set; } = 1275;

    /// <summary>
    /// 会心率所需系数2
    /// </summary>
    public int CriticalHitRightCoe { get; set; } = 1540;

    /// <summary>
    /// 计算元素抗性减免所需系数
    /// </summary>
    public int AntiElementCoe { get; set; } = 2266;

    /// <summary>
    /// 计算防御减免所需系数
    /// </summary>
    public int DefenseCoe { get; set; } = 10743;//目前元素抗性/防御减免系数不确定怎么去调整，先预埋

    #endregion

    /// <summary>
    /// 理论满命中，与实际测试会有微小差距
    /// </summary>
    public int FullHit => (int)Math.Ceiling(5 * FullHitCoe * 1.0 / 138 + Block);//133为方程 1.419*X/(3640+X)=0.05的向上取整值

    public object Clone()
    {
        return MemberwiseClone();
    }
}