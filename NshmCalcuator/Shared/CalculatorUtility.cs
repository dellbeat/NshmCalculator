using NshmCalculator.Shared.Models;

namespace NshmCalculator.Shared;

public class CalculatorUtility
{
    /// <summary>
    /// 计算增伤率
    /// </summary>
    /// <param name="baseInfo">基础面板属性，必要参数</param>
    /// <param name="attack">新增攻击</param>
    /// <param name="breakDefense">新增破防</param>
    /// <param name="elementAttack">新增元素攻击</param>
    /// <param name="restraint">新增首领克制数值</param>
    /// <param name="hit">新增命中</param>
    /// <param name="criticalHits">新增会心数值</param>
    /// <param name="criticalRate">新增会伤率</param>
    /// <returns>返回元组，分别为提升百分比和命中率</returns>
    public static (double, double) CalculateIncreaseRate(PlayerBaseInfo baseInfo, double attack = 0, int breakDefense = 0, int elementAttack = 0, int restraint = 0, double hit = 0, double criticalHits = 0, double criticalRate = 0)
    {
        (double, double) rate = (0, 0);//分别为提升百分比和命中率

        int defenseSubBase = baseInfo.EnemyDefense - baseInfo.PlayerBaseBreakDefense >= 0 ? baseInfo.EnemyDefense - baseInfo.PlayerBaseBreakDefense : 0; //由于破防超过防御部分无收益，故对破防数值进行预计算
        int defenseSubFull = baseInfo.EnemyDefense - baseInfo.PlayerBaseBreakDefense - breakDefense >= 0 ? baseInfo.EnemyDefense - baseInfo.PlayerBaseBreakDefense - breakDefense : 0;

        double b1 = (115 * baseInfo.PlayerBaseCriticalHits + 90) * 1.0 / (baseInfo.PlayerBaseCriticalHits + 940) / 100 + baseInfo.PlayerBaseZtCriticalHitsRate * 1.0 / 100;//暴击百分比
        double b2 = (95 + 143 * baseInfo.PlayerBaseHit * 1.0 / (baseInfo.PlayerBaseHit + 713) - 143 * baseInfo.EnemyBlock * 1.0 / (baseInfo.EnemyBlock + 713)) / 100;//命中值
        double b3 = (115 * (baseInfo.PlayerBaseCriticalHits + criticalHits) + 90) * 1.0 / (baseInfo.PlayerBaseCriticalHits + criticalHits + 940) / 100 + baseInfo.PlayerBaseZtCriticalHitsRate * 1.0 / 100;//新增后暴击百分比
        double b4 = (95 + 143 * (baseInfo.PlayerBaseHit + hit) * 1.0 / (baseInfo.PlayerBaseHit + hit + 713) - 143 * baseInfo.EnemyBlock * 1.0 / (baseInfo.EnemyBlock + 713)) / 100;//新增后命中值
        double b5 = b4 - b2;//新增命中百分比
        double b6 = (b2 * (1 + b3 * (baseInfo.PlayerBaseCriticalRate * 1.0 / 100 - 1)) + 0.5 - b2 / 2) / (b2 * (1 + b1 * (baseInfo.PlayerBaseCriticalRate * 1.0 / 100 - 1)) + 0.5 - b2 / 2) - 1;//暴击提升率
        double b7 = (2 * b5 * b1 * (baseInfo.PlayerBaseCriticalRate * 1.0 / 100 - 1) + b5) / (2 * b2 * b1 * (baseInfo.PlayerBaseCriticalRate * 1.0 / 100 - 1) + b2 + 1);//中间值
        double b8 = (2860 + baseInfo.EnemyDefense) * 1.0 / (2860 + defenseSubBase);//破防收益
        double b9 = (baseInfo.PlayerBaseAttack + baseInfo.PlayerBaseRestraint - baseInfo.EnemyAntiRestraint + 920) * 2860 * 1.0 / (2860 + defenseSubBase) + baseInfo.PlayerBaseElementAttack;//等效元素攻击
        double b10 = ((attack + restraint) * 2860 * 1.0 / (2860 + defenseSubBase) + elementAttack) / b9;//新增攻击率
        double b12 = ((baseInfo.PlayerBaseAttack + baseInfo.PlayerBaseRestraint - baseInfo.EnemyAntiRestraint + 920) * 2860 * 1.0 / (2860 + defenseSubBase) *
                      (2860 + defenseSubBase) / (2860 + defenseSubFull) + baseInfo.PlayerBaseElementAttack) / b9 - 1;//破防收益率
        double b13 = (b2 * (1 + b1 * (baseInfo.PlayerBaseCriticalRate * 1.0 / 100 + criticalRate * 1.0 / 100 - 1)) + 0.5 - b2 / 2) / (b2 * (1 + b1 * (baseInfo.PlayerBaseCriticalRate * 1.0 / 100 - 1)) + 0.5 - b2 / 2) - 1;//会伤收益率

        rate.Item1 = (b6 + 1) * (b7 + 1) * (1 + b10) * (1 + b12) * (1 + b13) - 1;
        rate.Item2 = b4;

        return rate;
    }

    /// <summary>
    /// 计算每点提升的收益
    /// </summary>
    /// <param name="baseInfo"></param>
    /// <returns>攻击克制,属性攻击,破防,命中,会心,会伤,气海,身法</returns>
    public static EarningRate CalculatePerGains(PlayerBaseInfo baseInfo)
    {
        EarningRate rate = new EarningRate
        {
            AttackAndRestraint = CalculateIncreaseRate(baseInfo, attack: 1).Item1,
            ElementAttack = CalculateIncreaseRate(baseInfo, elementAttack: 1).Item1,
            BreakDefense = CalculateIncreaseRate(baseInfo, breakDefense: 1).Item1,
            Hit = CalculateIncreaseRate(baseInfo, hit: 1).Item1,
            CriticalHits = CalculateIncreaseRate(baseInfo, criticalHits: 1).Item1,
            CriticalRate = CalculateIncreaseRate(baseInfo, criticalRate: 1).Item1
        };

        rate.Vitality = rate.AttackAndRestraint * 5 + rate.BreakDefense * 2;
        rate.Lightness = rate.Hit * 0.8 + rate.CriticalHits * 1.6;

        return rate;
    }
}