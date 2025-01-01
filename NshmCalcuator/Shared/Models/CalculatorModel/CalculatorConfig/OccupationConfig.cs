// ReSharper disable ClassNeverInstantiated.Global

using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Formula;
using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Params;
using NshmCalculator.Shared.Models.Interface;

namespace NshmCalculator.Shared.Models.CalculatorModel.CalculatorConfig;

/// <summary>
/// 夜银的身份收益计算器界面配置
/// </summary>
public class OccupationConfig : IGameConfig
{
    /// <summary>
    /// 原文件版本号
    /// </summary>
    public string Version { get; set; }
    
    public long InternalVersion { get; set; }

    /// <summary>
    /// 身份商品数据
    /// </summary>
    public OccupationGoods[] OccupationGoodsArray { get; set; }

    /// <summary>
    /// 身份收益公式数据
    /// </summary>
    public OccupationFormulas[] OccupationFormulaArray { get; set; }
    
    public string HelperText { get; set; }
}