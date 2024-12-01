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
    /// 首领数值项
    /// </summary>
    public FrontParamInfo[] BossParam { get; set; }
    
    //Todo：将开关设置/副本BUFF/伤害占比/覆盖率进行分组
    
    /// <summary>
    /// 当前加载的数据版本
    /// </summary>
    public long CurrentInternalVersion { get; set; } = -99999;
}