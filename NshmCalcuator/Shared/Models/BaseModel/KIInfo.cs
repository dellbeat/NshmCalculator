using System.ComponentModel.DataAnnotations;
using NshmCalculator.Shared.Models.Interface;

namespace NshmCalculator.Shared.Models.BaseModel;

/// <summary>
/// 内功详情
/// </summary>
public class KIInfo : ICharacterAttributes
{
    /// <summary>
    /// 内功名称
    /// </summary>
    public string? Name { get; set; }

    //TODO：考虑是否要加内功类型（图片可能暂时不太现实）

    /// <summary>
    /// 内功评分
    /// </summary>
    public int Score { get; set; }

    #region 数值

    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseStamina { get; set; }

    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseVitality { get; set; }

    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseStrength { get; set; }

    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseLightness { get; set; }

    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseFullAttack { get; set; }

    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseHalfAttack { get; set; }

    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseElementAttack { get; set; }

    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseRestraint { get; set; }

    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseCriticalHits { get; set; }

    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseBreakDefense { get; set; }

    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseHit { get; set; }

    #endregion

    /// <summary>
    /// 特性增伤百分比
    /// </summary>
    public double DamageIncreasePercent { get; set; }
}