using System.ComponentModel.DataAnnotations;
using NshmCalculator.Shared.Models.CalculatorModel;

namespace NshmCalculator.Shared.Models.PageModel;

public class SyTreatPageModel
{
    [ValidateComplexType]
    public TreatInfo BaseTreatInfo { get; set; } = new TreatInfo()
    {
        Attack = 8000,
        CalculateTreatNum = 6290,
        CriticalHits = 1500,
        ZtCriticalHitsRate = 3,
        CriticalDamageRate = 160
    };

    [ValidateComplexType]
    public TreatInfo DifferenceTreatInfo { get; set; } = new TreatInfo()
    {
        Attack = 8000,
        CalculateTreatNum = 6290,
        CriticalHits = 1500,
        ZtCriticalHitsRate = 3,
        CriticalDamageRate = 160
    };

    public TreatAttribution TreatAttribution { get; set; } = new();
    
    /// <summary>
    /// 新增攻击
    /// </summary>
    public int Attack { get; set; }
    
    /// <summary>
    /// 新增治疗
    /// </summary>
    public int TreatIntensity { get; set; }
    
    /// <summary>
    /// 新增会心
    /// </summary>
    public int CriticalHits { get; set; }
    
    /// <summary>
    /// 会伤百分比
    /// </summary>
    public double CriticalDamageRate { get; set; }
    
    /// <summary>
    /// 会心率
    /// </summary>
    public double CriticalHitsRate { get; set; }
    
    /// <summary>
    /// 当前TabIndex，用于控制计算行为
    /// </summary>
    public int ActiveIndex { get; set; }
}