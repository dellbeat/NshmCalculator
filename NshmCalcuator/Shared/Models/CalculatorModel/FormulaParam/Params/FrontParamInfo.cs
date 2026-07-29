using System.Text.Json.Serialization;
using NshmCalculator.Shared.Models.CalculatorModel.Enums;
using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.SpecialRule;

namespace NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Params;

/// <summary>
/// 界面动态参数配置
/// </summary>
public class FrontParamInfo
{
    /// <summary>
    /// 参数唯一标识符
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 标签名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 参数模式
    /// </summary>
    public ParamMode Mode { get; set; }

    /// <summary>
    /// 下拉框模式时的选项列表
    /// </summary>
    public string?[] Options { get; set; }

    /// <summary>
    /// 针对某些特殊选项进行数值关联的数组
    /// </summary>
    public double[]? Values { get; set; }

    /// <summary>
    /// 分组名，如为NULL则不作分组处理
    /// </summary>
    public string? GroupName { get; set; }

    /// <summary>
    /// 大类名称，用于确定参数渲染的顺序和区域
    /// </summary>
    public string CategoryName { get; set; }

    /// <summary>
    /// 补充说明
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 特殊参数规则
    /// </summary>
    public SpecialParamRule Rule { get; set; }
    
    /// <summary>
    /// 展示顺序，升序排序
    /// </summary>
    public int ShowIndex { get; set; }

    /// <summary>
    /// OCR 识别时是否排除该属性（true=不参与识别，如克制/抵御/会伤/会防/治疗强度）
    /// </summary>
    public bool OcrExclude { get; set; }

    /// <summary>
    /// OCR 识别时使用的自定义匹配前缀（如赛年词条用 "&lt;赛年&gt;"）。
    /// 为 null/空时使用 Name 作为正则锚点；非空时按"前缀+(可选属性名)+数值"格式累加。
    /// </summary>
    public string? OcrPrefix { get; set; }

    /// <summary>
    /// 用于绑定下拉列表的属性，无需序列化/反序列化
    /// </summary>
    [JsonIgnore]
    public string? StringValue { get; set; }

    /// <summary>
    /// 用于绑定数据的属性，无需序列化/反序列化
    /// </summary>
    [JsonIgnore]
    public double NumberValue { get; set; }
}