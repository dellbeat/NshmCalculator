namespace NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Result;

/// <summary>
/// 点名称类
/// </summary>
public class PointTitle
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    /// 需要参考的参数代号
    /// </summary>
    public string? RefParamCode { get; set; }
    
    /// <summary>
    /// 用于确定赋值规则的键值对
    /// </summary>
    public Dictionary<string,string> OptionDic { get; set; }
}