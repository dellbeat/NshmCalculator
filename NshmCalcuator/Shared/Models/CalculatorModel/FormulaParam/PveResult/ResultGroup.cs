namespace NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Result;

public class ResultGroup
{
    /// <summary>
    /// 组名称
    /// </summary>
    public string GroupName { get; set; }
    
    /// <summary>
    /// 所有线的集合
    /// </summary>
    public ResultLine[] ResultLines { get; set; }
}