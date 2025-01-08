// ReSharper disable ClassNeverInstantiated.Global
namespace NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Formula;

/// <summary>
/// 身份商品收益公式
/// </summary>
public class OccupationFormulas
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    public int ID { get; set; }    
    
    /// <summary>
    /// 商品名称
    /// </summary>
    public string GoodsName { get; set; }
    
    /// <summary>
    /// 计算公式
    /// </summary>
    public string Formula { get; set; }

    /// <summary>
    /// 计算公式参数列表
    /// </summary>
    public string[] FormulaParam { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }
    
    /// <summary>
    /// 收益
    /// </summary>
    public double Value { get; set; }
    
    /// <summary>
    /// 收藏标记
    /// </summary>
    public bool Favorite { get; set; }
}