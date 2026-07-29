// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CollectionNeverUpdated.Global

using System.Text.Json.Serialization;

namespace NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Params;

/// <summary>
/// 素鸿治疗计算器中保存的一条内功记录。
/// 仅持久化标题/备注/各词条填写数值/灵韵选择；总收益（<see cref="Score"/>）与评级（<see cref="GradeText"/>）
/// 由当前配置 + 页面疗承比复算，不序列化。
/// </summary>
public class SyTreatRecord
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 内功名称（允许重复）
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
    /// 各词条的填写数值（词条 Code -> 数值，仅包含保存时非 0 项）。
    /// 按词条 Code 存储以与配置改名解耦，即便后续配置的词条名称变化，仍可基于 Code 复算。
    /// </summary>
    public Dictionary<string, double> InputValues { get; set; } = new();

    /// <summary>
    /// 灵韵下拉框选中项名称（"无"表示不选；编辑弹窗可修改）。
    /// </summary>
    public string LingYunSelection { get; set; } = "无";

    /// <summary>
    /// 总收益（加载/重算时按当前配置 + 页面疗承比复算填充，不参与序列化）
    /// </summary>
    [JsonIgnore]
    public double Score { get; set; }

    /// <summary>
    /// 评级文本（S+/S/A/B/C，加载/重算时复算填充，不参与序列化）
    /// </summary>
    [JsonIgnore]
    public string GradeText { get; set; }
}
