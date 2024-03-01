using System.ComponentModel.DataAnnotations;

namespace NshmCalculator.Shared.Models.CalculatorModel;

public class TreatPlayerInfo
{
    #region 基础数据

    /// <summary>
    /// 玩家基础治疗强度
    /// </summary>
    [Required]
    [Range(0, 1000, ErrorMessage = "请保证基础治疗百分比在0.0-1000.0范围内")]
    public double PlayerBaseTreatPercent { get; set; }

    /// <summary>
    /// 玩家基础攻击力
    /// </summary>
    [Required]
    [Range(1, 100000, ErrorMessage = "请输入1至100000内的整数")]
    public int PlayerBaseAttack { get; set; }

    /// <summary>
    /// 玩家基础会心
    /// </summary>
    [Required]
    [Range(1, 100000, ErrorMessage = "请输入1至100000内的整数")]
    public int PlayerBaseCriticalHits { get; set; }

    /// <summary>
    /// 玩家基础会伤率
    /// </summary>
    [Required]
    [Range(0, 1000, ErrorMessage = "请保证会伤百分比在0.0-1000.0范围内")]
    public double PlayerBaseCriticalRate { get; set; }

    /// <summary>
    /// 玩家基础治疗会伤率
    /// </summary>
    [Required]
    [Range(0, 1000, ErrorMessage = "请保证会伤百分比在0.0-1000.0范围内")]
    public double PlayerBaseTreatCriticalRate { get; set; }

    /// <summary>
    /// 玩家周天会心率
    /// </summary>
    [Required]
    [Range(0, 5, ErrorMessage = "请保证周天会心百分比在0.0-5.0范围内")]
    public double PlayerBaseZtCriticalHitsRate { get; set; }

    #endregion

    #region 新增数据

    /// <summary>
    /// 新增攻击力
    /// </summary>
    [Required]
    [Range(0, 100000, ErrorMessage = "请输入1至100000内的整数")]
    public int IncreaseAttack { get; set; }

    /// <summary>
    /// 新增会伤百分比
    /// </summary>
    [Required]
    [Range(0, 10, ErrorMessage = "请保证新增会伤百分比在0.0-10.0范围内")]
    public double IncreaseCriticalRate { get; set; }

    /// <summary>
    /// 新增会心
    /// </summary>
    [Required]
    [Range(0, 100000, ErrorMessage = "请输入1至100000内的整数")]
    public int IncreaseCriticalHits { get; set; }

    /// <summary>
    /// 新增会心率
    /// </summary>
    [Required]
    [Range(0, 10, ErrorMessage = "请保证新增周天会心百分比在0.0-4.0范围内")]
    public double IncreaseZtCriticalRate { get; set; }

    #endregion

    /// <summary>
    /// 计算结果
    /// </summary>
    public double Result { get; set; }

    public TreatPlayerInfo()
    {
        PlayerBaseTreatPercent = 100;
        PlayerBaseAttack = 4000;
        PlayerBaseCriticalHits = 700;
        PlayerBaseCriticalRate = 170;
        PlayerBaseTreatCriticalRate = 135;
    }
}