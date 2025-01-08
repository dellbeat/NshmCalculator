using NshmCalculator.Shared.Models.CalculatorModel.Enums;

namespace NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Params;

public class ParamValue
{
    public double NumberValue { get; set; }

    public string? StringValue { get; set; }

    public bool NumberMode { get; set; }

    /// <summary>
    /// 供Json反序列化使用
    /// </summary>
    public ParamValue()
    {
    }

    public ParamValue(FrontParamInfo info)
    {
        NumberValue = info.NumberValue;
        StringValue = info.StringValue;
        NumberMode = info.Values != null || info.Mode is ParamMode.Number or ParamMode.Percent;
    }
}