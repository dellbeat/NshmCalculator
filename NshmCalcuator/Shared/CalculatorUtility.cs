using NshmCalculator.Shared.Models;
using NshmCalculator.Shared.Models.BaseModel;
using NshmCalculator.Shared.Models.CalculatorModel.KI;

namespace NshmCalculator.Shared;

public static class CalculatorUtility
{
    /// <summary>
    /// 分离玩家/敌方基础面板后的计算增伤率方法
    /// </summary>
    /// <param name="baseInfo">玩家基础面板属性，必要参数</param>
    /// <param name="enemyInfo">敌方基础面板属性</param>
    /// <param name="attack">新增攻击</param>
    /// <param name="breakDefense">新增破防</param>
    /// <param name="elementAttack">新增元素攻击</param>
    /// <param name="restraint">新增首领克制数值</param>
    /// <param name="hit">新增命中</param>
    /// <param name="criticalHits">新增会心数值</param>
    /// <param name="criticalRate">新增会伤率</param>
    /// <param name="fixHitMode">命中修复模式</param>
    /// <returns>返回元组，分别为提升百分比和命中率</returns>
    public static (double, double) CalculateIncreaseRate(PlayerBaseInfo baseInfo, EnemyInfo enemyInfo,
        double attack = 0, int breakDefense = 0, int elementAttack = 0, int restraint = 0, double hit = 0,
        double criticalHits = 0, double criticalRate = 0, bool fixHitMode = false)
    {
        (double, double) rate = (0, 0);//分别为提升百分比和命中率

        int defenseSubBase = enemyInfo.Defense - baseInfo.BaseBreakDefense >= 0 ? enemyInfo.Defense - baseInfo.BaseBreakDefense : 0; //由于破防超过防御部分无收益，故对破防数值进行预计算
        int defenseSubFull = enemyInfo.Defense - baseInfo.BaseBreakDefense - breakDefense >= 0 ? enemyInfo.Defense - baseInfo.BaseBreakDefense - breakDefense : 0;
        double baseHit = fixHitMode ? baseInfo.BaseHit > enemyInfo.FullHit ? enemyInfo.FullHit : baseInfo.BaseHit : baseInfo.BaseHit;
        double addedHit = fixHitMode ? (baseInfo.BaseHit + hit > enemyInfo.FullHit ? enemyInfo.FullHit : baseInfo.BaseHit + hit) : baseInfo.BaseHit + hit;

        double b1 = (115 * (baseInfo.BaseCriticalHits - enemyInfo.AntiCriticalHits) + 90) * 1.0 / (baseInfo.BaseCriticalHits - enemyInfo.AntiCriticalHits + 940) / 100 + baseInfo.BaseZtCriticalHitsRate * 1.0 / 100;//暴击百分比
        double b2 = (95 + 143 * baseHit * 1.0 / (baseHit + 713) - 143 * enemyInfo.Block * 1.0 / (enemyInfo.Block + 713)) / 100;//命中值
        double b3 = (115 * (baseInfo.BaseCriticalHits - enemyInfo.AntiCriticalHits + criticalHits) + 90) * 1.0 / (baseInfo.BaseCriticalHits - enemyInfo.AntiCriticalHits + criticalHits + 940) / 100 + baseInfo.BaseZtCriticalHitsRate * 1.0 / 100;//新增后暴击百分比
        double b4 = (95 + 143 * addedHit * 1.0 / (addedHit + 713) - 143 * enemyInfo.Block * 1.0 / (enemyInfo.Block + 713)) / 100;//新增后命中值
        double b5 = b4 - b2;//新增命中百分比
        double b6 = (b2 * (1 + b3 * (baseInfo.BaseCriticalRate * 1.0 / 100 - 1)) + 0.5 - b2 / 2) / (b2 * (1 + b1 * (baseInfo.BaseCriticalRate * 1.0 / 100 - 1)) + 0.5 - b2 / 2) - 1;//暴击提升率
        double b7 = (2 * b5 * b1 * (baseInfo.BaseCriticalRate * 1.0 / 100 - 1) + b5) / (2 * b2 * b1 * (baseInfo.BaseCriticalRate * 1.0 / 100 - 1) + b2 + 1);//中间值
        double b8 = (2860 + enemyInfo.Defense) * 1.0 / (2860 + defenseSubBase);//破防收益
        double b9 = (baseInfo.BaseAttack + baseInfo.BaseRestraint - enemyInfo.AntiRestraint + 920) * 2860 * 1.0 / (2860 + defenseSubBase) + baseInfo.BaseElementAttack - enemyInfo.AntiElementAttack;//等效元素攻击
        double b10 = ((attack + restraint) * 2860 * 1.0 / (2860 + defenseSubBase) + elementAttack) / b9;//新增攻击率
        double b12 = ((baseInfo.BaseAttack + baseInfo.BaseRestraint - enemyInfo.AntiRestraint + 920) * 2860 * 1.0 / (2860 + defenseSubBase) *
                      (2860 + defenseSubBase) / (2860 + defenseSubFull) + baseInfo.BaseElementAttack - enemyInfo.AntiElementAttack) / b9 - 1;//破防收益率
        double b13 = (b2 * (1 + b1 * (baseInfo.BaseCriticalRate * 1.0 / 100 + criticalRate * 1.0 / 100 - 1)) + 0.5 - b2 / 2) / (b2 * (1 + b1 * (baseInfo.BaseCriticalRate * 1.0 / 100 - 1)) + 0.5 - b2 / 2) - 1;//会伤收益率

        rate.Item1 = (b6 + 1) * (b7 + 1) * (1 + b10) * (1 + b12) * (1 + b13) - 1;
        rate.Item2 = fixHitMode ? baseInfo.BaseHit + hit >= enemyInfo.FullHit ? 1 : b4 : b4;

        return rate;
    }

    /// <summary>
    /// 计算每点提升的收益
    /// </summary>
    /// <param name="baseInfo">玩家面板</param>
    /// <param name="enemyInfo">敌方数据</param>
    /// <param name="deliveryData">投放数据，目前为定值</param>
    /// <returns>攻击克制,属性攻击,破防,命中,会心,会伤,气海,身法</returns>
    public static KiEarningRate CalculatePerGains(PlayerBaseInfo baseInfo, EnemyInfo enemyInfo, DeliveryData deliveryData)
    {
        KiEarningRate rate = new KiEarningRate
        {
            AttackAndRestraint = CalculateIncreaseRate(baseInfo, enemyInfo, attack: deliveryData.Attack).Item1 / deliveryData.Attack,
            ElementAttack = CalculateIncreaseRate(baseInfo, enemyInfo, elementAttack: deliveryData.ElementAttack).Item1 / deliveryData.ElementAttack,
            BreakDefense = CalculateIncreaseRate(baseInfo, enemyInfo, breakDefense: deliveryData.BreakDefense).Item1 / deliveryData.BreakDefense,
            Hit = CalculateIncreaseRate(baseInfo, enemyInfo, hit: deliveryData.Hit, fixHitMode: true).Item1 / deliveryData.Hit,
            CriticalHits = CalculateIncreaseRate(baseInfo, enemyInfo, criticalHits: deliveryData.CriticalHits).Item1 / deliveryData.CriticalHits,
            CriticalRate = CalculateIncreaseRate(baseInfo, enemyInfo, criticalRate: deliveryData.CriticalRate).Item1 / deliveryData.CriticalRate
        };

        rate.Vitality = rate.AttackAndRestraint * 5 + rate.BreakDefense * 2;
        rate.Lightness = rate.Hit * 0.8 + rate.CriticalHits * 1.6;

        return rate;
    }

    /// <summary>
    /// 获取无首领加成的伤害
    /// </summary>
    /// <param name="attackOfPlayer">玩家攻击</param>
    /// <param name="restraintNum">玩家克制数值</param>
    /// <param name="elementAttackOfPlayer">玩家元素攻击</param>
    /// <param name="breakDefenseOfPlayer">玩家破防值</param>
    /// <param name="defenseOfMonster">敌方防御</param>
    /// <param name="resistOfMonster">敌方抵御</param>
    /// <param name="rate1">系数1</param>
    /// <param name="rate2">系数2</param>
    /// <param name="remainAirShield">剩余气盾，由于PVE目前暂不考虑该参数，故默认为0</param>
    public static double CalculateBaseDamage(int attackOfPlayer, int restraintNum, int elementAttackOfPlayer, int breakDefenseOfPlayer, int defenseOfMonster, int resistOfMonster, double rate1, double rate2, int remainAirShield = 0)
    {
        double resistanceRemission = resistOfMonster * 1.0 / (resistOfMonster + 530);//敌方（怪物）抗性减免
        int remainDefense = defenseOfMonster - breakDefenseOfPlayer;//敌方剩余防御，理论不会小于0，不做特别判断
        if (remainDefense < 0)
        {
            remainDefense = 0;
        }
        double defenseRemission = remainDefense * 1.0 / (remainDefense + 2860);//防御减免

        double baseDamage = ((rate1 + rate2 * (attackOfPlayer - remainAirShield + restraintNum - resistOfMonster)) * (1 - defenseRemission) + rate2 * elementAttackOfPlayer * (1 - resistanceRemission));//无首领克制加成的伤害

        return baseDamage;
    }

    /// <summary>获取未会心伤害</summary>
    /// <param name="baseDamage">无首领克制的伤害</param>
    /// <param name="restrainedRate">首领克制百分比</param>
    public static double CalculateNonCriticalDamage(double baseDamage, double restrainedRate)
    {
        return baseDamage * (1 + restrainedRate);
    }


    /// <summary>
    /// 获取会心伤害
    /// </summary>
    /// <param name="nonCriticalDamage">未会心伤害</param>
    /// <param name="hitNum">命中</param>
    /// <param name="blockOfMonster">敌方格挡</param>
    /// <param name="criticalSubRate">会心伤害百分比-100%</param>
    /// <param name="criticalHit">会心</param>
    /// <param name="criticalDefenseOfMonster">敌方会心抵抗</param>
    /// <param name="extraCriticalRate">内功提供的额外会心率</param>
    /// <param name="calCriticalRate">计算的会心率结果</param>
    public static double CalculateCriticalDamage(double nonCriticalDamage, int hitNum, int blockOfMonster, double criticalSubRate, int criticalHit, int criticalDefenseOfMonster, double extraCriticalRate, out double calCriticalRate)
    {
        double panelHitRateOfPlayer = (143 * hitNum * 1.0 / (hitNum + 713) / 100 is double panelNum && panelNum > 1 ? 1 : panelNum);//玩家[攻击方]面板命中率
        double panelDefenseRateOfMonster = 143 * 1.0 * blockOfMonster / (blockOfMonster + 713) / 100;//敌方[受击方]面板格挡率Da
        double hitRateOfPlayer = 0.95 + panelHitRateOfPlayer - panelDefenseRateOfMonster;//玩家对敌方命中率
        if (hitRateOfPlayer > 1)
        {
            hitRateOfPlayer = 1;
        }
        int remainCritical = (criticalHit - criticalDefenseOfMonster);//剩余会心
        double criticalRate = (115 * remainCritical + 90) * 1.0 / (remainCritical + 940) / 100 + extraCriticalRate;//会心率
        calCriticalRate = criticalRate;
        double criticalDamage = nonCriticalDamage * hitRateOfPlayer * (1 + criticalSubRate * criticalRate) + 0.5 * nonCriticalDamage * (1 - hitRateOfPlayer);//会心伤害【未计算技能倍数】

        return criticalDamage;
    }
}