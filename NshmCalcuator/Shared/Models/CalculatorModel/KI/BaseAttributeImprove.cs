namespace NshmCalculator.Shared.Models.CalculatorModel.KI;

/// <summary>
/// 最大基础词条加成参考
/// </summary>
public class BaseAttributeImprove
{
    /// <summary>
    /// 词条
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// 最大值
    /// </summary>
    public int MaxValue { get; set; }
    
    /// <summary>
    /// 攻击分
    /// </summary>
    public double AttackScore { get; set; }
    
    /// <summary>
    /// 提升率
    /// </summary>
    public double ImprovePercent { get; set; }
}