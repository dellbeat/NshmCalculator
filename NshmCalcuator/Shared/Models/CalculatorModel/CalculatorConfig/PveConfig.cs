using NshmCalculator.Shared.Models.Interface;

namespace NshmCalculator.Shared.Models.CalculatorModel.CalculatorConfig;

public class PveConfig: IGameConfig
{
	/// <summary>
	/// 页面中所使用的公式
	/// </summary>
	public PveFormula[] InternalFormulas { get; set; }
    
    /// <summary>
    /// 组别列表，确定界面渲染顺序
    /// </summary>
    public string[] CategoryArray { get; set; }
    
    /// <summary>
    /// 前端选项
    /// </summary>
    public FrontParamInfo[] FrontParamInfoArray { get; set; }
    
    /// <summary>
    /// 结果公式
    /// </summary>
    public PveFormula[] ResultFormulas { get; set; }
    
    public long InternalVersion { get; set; } = -99999;
    
    public string HelperText { get; set; }
}