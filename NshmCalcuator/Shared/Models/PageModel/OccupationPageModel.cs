using System.ComponentModel.DataAnnotations;
using NshmCalculator.Shared.Models.CalculatorModel;
using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Formula;
using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Params;

namespace NshmCalculator.Shared.Models.PageModel;

public class OccupationPageModel
{
    /// <summary>
    /// 界面上存储的商品数据及价格配置
    /// </summary>
    [ValidateComplexType]
    public OccupationGoods[] GoodsArray { get; set; } = [];
    
    /// <summary>
    /// 界面上存储的公式配置及计算结果
    /// </summary>
    public OccupationFormulas[] FormulasArray{ get; set; }= [];
    

    /// <summary>
    /// 当前加载的内部版本
    /// </summary>
    public long CurrentInternalVersion { get; set; } = -99999;
}