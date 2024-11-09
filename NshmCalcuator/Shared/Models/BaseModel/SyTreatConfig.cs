namespace NshmCalculator.Shared.Models.BaseModel;

/// <summary>
/// 师尹的素问治疗计算器页面的数据配置
/// </summary>
public class SyTreatConfig
{
    /// <summary>
    /// 原文件版本，对应界面上的引用
    /// </summary>
    public string Version { get; set; }

    #region 系数
    
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
    /// 是否读取/保存转换疗强实体，作为兼容性选项，后续不再处理
    /// </summary>
    public bool HandleTreatAttribution { get; set; }
}