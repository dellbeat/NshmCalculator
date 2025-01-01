using System.ComponentModel.DataAnnotations;

// ReSharper disable ClassNeverInstantiated.Global

namespace NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Params;

/// <summary>
/// 身份玩法中的商品
/// </summary>
public class OccupationGoods
{
    /// <summary>
    /// 商品名称
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// 所属名称
    /// </summary>
    public string BelongGroup { get; set; }
    
    /// <summary>
    /// 商品价格
    /// </summary>
    [Range(1, 100000, ErrorMessage = "卖东西不能倒贴钱")]
    public int Price { get; set; }
    
    /// <summary>
    /// 公式代码,唯一标识
    /// </summary>
    public string Code { get; set; }
}