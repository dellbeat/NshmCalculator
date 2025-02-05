using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using NshmCalculator.Shared.Models.CalculatorModel;

namespace NshmCalculator.Shared.Models.PageModel;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public class SyTreatPageModel
{
    [ValidateComplexType]
    public TreatInfo BaseTreatInfo { get; set; } = new TreatInfo()
    {
        Attack = 8000,
        TreatIntensity = 6290,
        CriticalHits = 1500,
        ZtCriticalHitsRate = 3,
        CriticalDamageRate = 160
    };

    [ValidateComplexType]
    public TreatInfo DifferenceTreatInfo { get; set; } = new TreatInfo()
    {
        Attack = 8000,
        TreatIntensity = 6290,
        CriticalHits = 1500,
        ZtCriticalHitsRate = 3,
        CriticalDamageRate = 160
    };

    /// <summary>
    /// 转换治疗计算界面配置,仅在读取时进行赋值
    /// </summary>
    public FieldInfo[] TransTreatFields { get; set; }
    
    /// <summary>
    /// 转换治疗计算数值键值对，在计算时保存，在读取时恢复
    /// </summary>
    public Dictionary<string,int> TransTreatDictionary { get; set; } = new();
    
    /// <summary>
    /// PVP评分计算界面配置,仅在读取时进行赋值
    /// </summary>
    public FieldInfo[] ScoreFields { get; set; }
    
    /// <summary>
    /// PVP评分计算数值键值对，在计算时保存，在读取时恢复
    /// </summary>
    public Dictionary<string,int> ScoreFieldsDictionary { get; set; } = new();

    /// <summary>
    /// 疗承比
    /// </summary>
    public double TreatDefenseRatio { get; set; } = 1.5;
    
    /// <summary>
    /// 显示在界面上的特殊分数
    /// </summary>
    public string SpecialScoreText { get; set; }
    
    #region 变化计算面板

    /// <summary>
    /// 新增攻击
    /// </summary>
    public int Attack { get; set; }
    
    /// <summary>
    /// 新增治疗
    /// </summary>
    public int TreatIntensity { get; set; }
    
    /// <summary>
    /// 新增会心
    /// </summary>
    public int CriticalHits { get; set; }

    /// <summary>
    /// 新增会伤百分比
    /// </summary>
    public double CriticalDamageRate { get; set; }

    /// <summary>
    /// 新增会心率
    /// </summary>
    public double CriticalHitsRate { get; set; }

    /// <summary>
    /// 新增后面板计算会心率
    /// </summary>
    public double CalculateCriticalHitsRate { get; set; }

    /// <summary>
    /// 新增后面板治疗量
    /// </summary>
    public double CalculateTreatNum { get; set; }

    #endregion

    /// <summary>
    /// 选择的灵韵分键值对
    /// </summary>
    public string SelectedKey { get; set; } 

    /// <summary>
    /// 当前TabIndex，用于控制计算行为
    /// </summary>
    public int ActiveIndex { get; set; }

    /// <summary>
    /// 是否为比较计算，以控制渲染的元素
    /// </summary>
    public bool ConvertMode { get; set; }

    /// <summary>
    /// 治疗量增值
    /// </summary>
    public double TreatGrowthNum { get; set; }

    /// <summary>
    /// 治疗量增值百分比
    /// </summary>
    public double TreatGrowthPercent { get; set; }

    /// <summary>
    /// 转换疗强
    /// </summary>
    public double ConvertTreatNum { get; set; }
    
    public double Score { get; set; }
    
    /// <summary>
    /// 配置的内部代号
    /// </summary>
    public long InternalVersion { get; set; }
}