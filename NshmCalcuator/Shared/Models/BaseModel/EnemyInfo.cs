using System.ComponentModel.DataAnnotations;

namespace NshmCalculator.Shared.Models.BaseModel;

/// <summary>
/// 敌方基础数值
/// </summary>
public class EnemyInfo
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

    /// <summary>
    /// 满命中
    /// </summary>
    public int FullHit => Convert.ToInt32(Math.Floor((105524 * Block + 2541845) * 1.0 / (98394 - 5 * Block)));
}