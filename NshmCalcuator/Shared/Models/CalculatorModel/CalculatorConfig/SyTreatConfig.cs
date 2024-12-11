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
    
    //乘数：与其他数乘/除；增数，与其他数加/减
    
    /// <summary>
    /// 会心乘数
    /// </summary>
    public double CriticalHitsMult { get; set; }
    
    /// <summary>
    /// 会心增数1
    /// </summary>
    public double CriticalHitsAddition1 { get; set; }
    
    /// <summary>
    /// 会心增数2
    /// </summary>
    public double CriticalHitsAddition2 { get; set; }
    
    /// <summary>
    /// 治疗强度增数
    /// </summary>
    public double TreatIntensityAddition { get; set; }

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