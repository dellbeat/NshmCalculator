using System.ComponentModel.DataAnnotations;
using System.Reflection;
using NshmCalculator.Shared.Models.Interface;

namespace NshmCalculator.Shared.Models.BaseModel;

/// <summary>
/// 内功详情
/// </summary>
public class KIInfo : ICharacterAttributes
{
    /// <summary>
    /// 唯一识别符
    /// </summary>
    public int ID { get; set; } = -1;

    /// <summary>
    /// 内功名称
    /// </summary>
    [Required(ErrorMessage = "请给内功一个称呼")]
    public string? Name { get; set; }

    //TODO：考虑是否要加内功类型（图片可能暂时不太现实）

    /// <summary>
    /// 内功评分
    /// </summary>
    public int? RateScore { get; set; }

    #region 数值

    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseStamina { get; set; }

    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseVitality { get; set; }

    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseStrength { get; set; }

    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseLightness { get; set; }

    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseFullAttack { get; set; }

    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseHalfAttack { get; set; }

    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseElementAttack { get; set; }

    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseRestraint { get; set; }

    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseCriticalHits { get; set; }

    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseBreakDefense { get; set; }

    [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
    public int IncreaseHit { get; set; }

    #endregion

    public string NumberSummary { get; set; }

    /// <summary>
    /// 特性增伤百分比
    /// </summary>
    public double DamageIncreasePercent { get; set; }
    
    /// <summary>
    /// 攻击分
    /// </summary>
    public double AttackScore { get; set; }

    private readonly PropertyInfo[] _infos;
    private static Dictionary<string,string> _nameDic { get; set; }
    
    static KIInfo()
    {
        _nameDic = new Dictionary<string, string>
        {
            { nameof(IncreaseStamina), "耐力" },
            { nameof(IncreaseVitality), "根骨" },
            { nameof(IncreaseStrength), "气海力量" },
            { nameof(IncreaseLightness), "身法" },
            { nameof(IncreaseFullAttack), "攻击力" },
            { nameof(IncreaseHalfAttack), "大小攻" },
            { nameof(IncreaseElementAttack), "属性攻击" },
            { nameof(IncreaseRestraint), "首领克制" },
            { nameof(IncreaseCriticalHits), "会心" },
            { nameof(IncreaseBreakDefense), "破防" },
            { nameof(IncreaseHit), "命中" }
        };
    }
    
    public KIInfo()
    {
        _infos= GetType().GetProperties().Where(p => p.PropertyType == typeof(int) && p.Name != "ID").ToArray();
    }

    public KIInfo(ICharacterAttributes attributes) : this()
    {
        IncreaseStamina = attributes.IncreaseStamina;
        IncreaseVitality = attributes.IncreaseVitality;
        IncreaseStrength = attributes.IncreaseStrength;
        IncreaseLightness = attributes.IncreaseLightness;
        IncreaseFullAttack = attributes.IncreaseFullAttack;
        IncreaseHalfAttack = attributes.IncreaseHalfAttack;
        IncreaseElementAttack = attributes.IncreaseElementAttack;
        IncreaseRestraint = attributes.IncreaseRestraint;
        IncreaseCriticalHits = attributes.IncreaseCriticalHits;
        IncreaseBreakDefense = attributes.IncreaseBreakDefense;
        IncreaseHit = attributes.IncreaseHit;
    }

    public void UpdateSummary()
    {
        NumberSummary = "";
        int validCount = 0;
        foreach (var propertyInfo in _infos)
        {
            if (propertyInfo.GetValue(this) is int number and > 0)
            {
                if (validCount > 0 && validCount % 3 == 0)
                {
                    NumberSummary += "\r\n";
                }
                else if (!string.IsNullOrEmpty(NumberSummary))
                {
                    NumberSummary += "/";
                }
                NumberSummary += $"{_nameDic[propertyInfo.Name]}:{number}";
                validCount++;
            }
        }

        if (string.IsNullOrEmpty(NumberSummary))
        {
            NumberSummary = "无数值";
        }
    }
}