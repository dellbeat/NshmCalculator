// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CollectionNeverUpdated.Global

using System.Text.Json.Serialization;

namespace NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Params;

/// <summary>
/// 升级内功词条计算器中保存的一条内功记录。
/// 仅持久化标题/备注/各属性的填写数值；实际收益率（<see cref="TotalRate"/>）由当前配置复算，不序列化。
/// </summary>
public class KiUpdateRecord
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 内功标题（允许重复）
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 内功备注
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// 创建时间（用于排序/展示）
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 各属性的填写数值（属性名 -> 数值，仅包含已填写项）。
    /// 按属性名存储以与配置解耦，即便后续配置的上限/收益率更新，仍可基于当前配置复算。
    /// </summary>
    public Dictionary<string, double> InputValues { get; set; } = new();

    /// <summary>
    /// 实际收益率（加载或编辑时按当前配置复算填充，不参与序列化）
    /// </summary>
    [JsonIgnore]
    public double TotalRate { get; set; }
}
