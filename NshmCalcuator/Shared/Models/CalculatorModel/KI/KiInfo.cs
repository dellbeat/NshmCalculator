namespace NshmCalculator.Shared.Models.CalculatorModel.KI;

public class KiInfo
{
    /// <summary>
    /// 内功名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 内功评分
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// 特性增伤百分比
    /// </summary>
    public double DamageIncreasePercent { get; set; }
}