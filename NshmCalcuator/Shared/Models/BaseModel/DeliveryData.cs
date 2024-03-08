namespace NshmCalculator.Shared.Models.BaseModel;

/// <summary>
/// 属性投放数据（后续考虑调整结构）
/// </summary>
public class DeliveryData
{
    /// <summary>
    /// 攻击
    /// </summary>
    public int Attack { get; set; } = 80;
    /// <summary>
    /// 克制
    /// </summary>
    public int Restraint { get; set; } = 60;
    /// <summary>
    /// 元素攻击
    /// </summary>
    public int ElementAttack { get; set; } = 47;
    /// <summary>
    /// 破防
    /// </summary>
    public int BreakDefense { get; set; } = 117;
    /// <summary>
    /// 命中
    /// </summary>
    public int Hit { get; set; } = 34;
    /// <summary>
    /// 会心
    /// </summary>
    public int CriticalHits { get; set; } = 41;
    /// <summary>
    /// 会心率
    /// </summary>
    public int CriticalRate { get; set; } = 1;
}