using System.ComponentModel.DataAnnotations;

namespace NshmCalculator.Shared.Models.CalculatorModel;

public class TreatInfo
{
    #region 基础数值

    /// <summary>
    /// 面板攻击
    /// </summary>
    [Required(ErrorMessage = "请输入整数")]
    [Range(0, 100000, ErrorMessage = "请保证面板攻击在0-100000范围内")]
    public int Attack { get; set; }
    
    /// <summary>
    /// 基础治疗强度
    /// </summary>
    [Required(ErrorMessage = "请输入整数")]
    [Range(0, 100000, ErrorMessage = "请保证治疗强度在0-100000范围内")]
    public int TreatIntensity { get; set; }
    
    /// <summary>
    /// 基础会心
    /// </summary>
    [Required(ErrorMessage = "请输入整数")]
    [Range(0, 100000, ErrorMessage = "请保证会心在0-100000范围内")]
    public int CriticalHits { get; set; }
    
    /// <summary>
    /// 周天会心率（注意为百分比，需要做处理）
    /// </summary>
    [Required(ErrorMessage = "请直接输入数字，无需百分号")]
    [Range(0, 5, ErrorMessage = "请保证周天会心百分比在0.0-5.0范围内")]
    public double ZtCriticalHitsRate { get; set; }

    /// <summary>
    /// 会伤率
    /// </summary>
    [Required(ErrorMessage = "请直接输入数字，无需百分号")]
    [Range(0, 1000, ErrorMessage = "请保证会伤率在0.0-1000.0范围内")]
    public double CriticalDamageRate { get; set; }

    #endregion

    /// <summary>
    /// 会心率增量（不在页面上展示，不用考虑百分比格式）
    /// </summary>
    public double ExtraCriticalHitsRate { get; set; }

    #region 计算结果区域

    /// <summary>
    /// 基础面板计算会心率
    /// </summary>
    public double CalculateCriticalHitsRate { get; set; }
    
    /// <summary>
    /// 基础面板治疗量
    /// </summary>
    public double CalculateTreatNum { get; set; }

    #endregion
}

public class TreatAttribution
{
    /// <summary>
    /// 攻击
    /// </summary>
    public int Attack { get; set; }
    
    /// <summary>
    /// 气海
    /// </summary>
    public int Strength { get; set; }
    
    /// <summary>
    /// 大小攻总和
    /// </summary>
    public int HalfAttackSum { get; set; }
    
    /// <summary>
    /// 破防
    /// </summary>
    public int BreakDefense { get; set; }
    
    /// <summary>
    /// 全元素攻击
    /// </summary>
    public int ElementAttack { get; set; }
    
    /// <summary>
    /// 首领克制
    /// </summary>
    public int MonsterRestraint { get; set; }
    
    /// <summary>
    /// 命中
    /// </summary>
    public int Hit { get; set; }
    
    /// <summary>
    /// 忽视元素抗
    /// </summary>
    public int IgnoreElementDefense { get; set; }
    
    /// <summary>
    /// 流派克制
    /// </summary>
    public int ProfessionRestraint { get; set; }
}