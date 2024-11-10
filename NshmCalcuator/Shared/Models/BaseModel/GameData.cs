using System.Text.Json.Serialization;
using NshmCalculator.Shared.Models.CalculatorModel;
using NshmCalculator.Shared.Models.CalculatorModel.KI;

namespace NshmCalculator.Shared.Models.BaseModel;

/// <summary>
/// 后续加载的唯一数据类
/// </summary>
public class GameData
{
    /// <summary>
    /// 木桩数据
    /// </summary>
    [JsonInclude]
    public Dictionary<string, EnemyInfo> EnemyData { get; set; }
    
    /// <summary>
    /// 最大基础词条加成数据（已废弃，仅供兼容用）
    /// </summary>
    [JsonInclude]
    public BaseAttributeImprove[] ImproveScore { get; set; }
    
    /// <summary>
    /// 师尹素问治疗计算器计算系数配置
    /// </summary>
    [JsonInclude]
    public SyTreatConfig[] SyTreatConfig { get; set; }

    public GameData()
    {
        EnemyData = new Dictionary<string, EnemyInfo>();
        ImproveScore = [];
        SyTreatConfig = [];
    }
}