// ReSharper disable ClassNeverInstantiated.Global
namespace NshmCalculator.Shared.Models.CalculatorModel.CalculatorConfig;

/// <summary>
/// 夜银的身份收益计算器界面配置
/// </summary>
public class OccupationConfig
{
    /// <summary>
    /// 原文件版本号
    /// </summary>
    public string Version { get; set; }
    
    /// <summary>
    /// 配置内部版本号，以日期+时间进行标识
    /// </summary>
    public long InternalVersion { get; set; }
    
    /// <summary>
    /// 身份商品数据
    /// </summary>
    public OccupationGoods[] OccupationGoodsArray { get; set; }

    /// <summary>
    /// 身份收益公式数据
    /// </summary>
    public OccupationFormulas[] OccupationFormulaArray { get; set; }
}