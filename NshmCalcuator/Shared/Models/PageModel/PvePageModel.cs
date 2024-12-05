using NshmCalculator.Shared.Models.CalculatorModel;

namespace NshmCalculator.Shared.Models.PageModel;

public class PvePageModel
{
    /// <summary>
    /// 页面中所使用的公式
    /// </summary>
    public PveFormula[] PveFormulas { get; set; }
    
    /// <summary>
    /// 角色基础数值项
    /// </summary>
    public FrontParamInfo[] BaseParam { get; set; }
    
    /// <summary>
    /// 开关设置项
    /// </summary>
    public FrontParamInfo[] SwitchParam { get; set; }
    
    /// <summary>
    /// 增益类选项
    /// </summary>
    public FrontParamInfo[] BuffParam { get; set; }
    
    /// <summary>
    /// 首领数值项
    /// </summary>
    public FrontParamInfo[] BossParam { get; set; }
    
    /// <summary>
    /// 伤害占比项
    /// </summary>
    public FrontParamInfo[] DamageParam { get; set; }
    
    /// <summary>
    /// 覆盖率
    /// </summary>
    public FrontParamInfo[] CoverageParam { get; set; }
    
    /// <summary>
    /// 当前加载的数据版本
    /// </summary>
    public long CurrentInternalVersion { get; set; } = -99999;
}