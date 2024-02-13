namespace NshmCalculator.Shared.Models;

public class EarningRate
{
    /// <summary>
    /// 每点攻击及首领克制收益
    /// </summary>
    public double AttackAndRestraint { get; init; }

    /// <summary>
    /// 每点属性攻击收益
    /// </summary>
    public double ElementAttack { get; init; }

    /// <summary>
    /// 每点破防收益
    /// </summary>
    public double BreakDefense { get; init; }

    /// <summary>
    /// 每点命中的收益
    /// </summary>
    public double Hit { get; init; }

    /// <summary>
    /// 每点会心的收益
    /// </summary>
    public double CriticalHits { get; init; }

    /// <summary>
    /// 每百分比会伤收益
    /// </summary>
    public double CriticalRate { get; init; }

    /// <summary>
    /// 每点气海收益
    /// </summary>
    public double Vitality { get; set; }

    /// <summary>
    /// 每点身法收益
    /// </summary>
    public double Lightness { get; set; }
}