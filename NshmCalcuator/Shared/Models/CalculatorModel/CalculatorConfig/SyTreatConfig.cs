using NshmCalculator.Shared.Models.Interface;

namespace NshmCalculator.Shared.Models.CalculatorModel.CalculatorConfig;

/// <summary>
/// 师尹的素问治疗计算器页面的数据配置
/// </summary>
public class SyTreatConfig: IGameConfig
{
    /// <summary>
    /// 原文件版本，对应界面上的引用
    /// </summary>
    public string Version { get; set; }

    #region 治疗量计算系数

    /// <summary>
    /// 会心50%临界评分（对应4.1.1表格I10公式常量2290）：会心率 = 1/(1+e^(1−会心/该值))
    /// </summary>
    public double CriticalRating50 { get; set; }

    #endregion

    /// <summary>
    /// 转换疗强参数列表
    /// </summary>
    public FieldInfo[] TransTreatFields { get; set; } = null;

    /// <summary>
    /// 属性词条数值参数列表
    /// </summary>
    public FieldInfo[] ScoreFields { get; set; } = null;

    /// <summary>
    /// 属性词条分类排序
    /// </summary>
    public string[] ScoreFieldsGroupArray { get; set; } = null;
    
    /// <summary>
    /// 灵韵分映射
    /// </summary>
    public Dictionary<string,string> SpecialScoreDictionary { get; set; } = null;
    
    /// <summary>
    /// 需要除以疗承比的列表
    /// </summary>
    public string[] ExceptSpecialScoreArray { get; set; } = null;
    
    public long InternalVersion { get; set; }
    
    public string HelperText { get; set; }
}