namespace NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Result;

/// <summary>
/// 表示由一串点组成的线
/// </summary>
public class ResultLine
{
    /// <summary>
    /// 数据集合
    /// </summary>
    public ResultItem[] ResultItems { get; set; }
    
    /// <summary>
    /// 曲线名称
    /// </summary>
    public string LineName { get; set; }
}