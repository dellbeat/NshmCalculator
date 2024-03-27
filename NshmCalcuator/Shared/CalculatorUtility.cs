using NshmCalculator.Shared.Models.BaseModel;
using NshmCalculator.Shared.Models.BaseModel.Enums;
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
    /// <param name="breakAirShield">新增破盾</param>
    /// <param name="criticalRate">新增会伤率</param>
    /// <param name="fixHitMode">命中限制模式</param>
    /// <param name="version">算法版本</param>
    /// <returns>返回元组，分别为提升百分比和命中率</returns>
    public static (double, double) CalculateIncreaseRate(PlayerBaseInfo baseInfo, EnemyInfo enemyInfo,
        double attack = 0, int breakDefense = 0, int elementAttack = 0, double restraint = 0, double hit = 0,
        double criticalHits = 0, double criticalRate = 0, bool fixHitMode = false,
        HitCalculateVersion version = HitCalculateVersion.Version13)
    {
        (double, double) rate = (0, 0); //分别为提升百分比和命中率

        int defenseSubBase = enemyInfo.Defense - baseInfo.BaseBreakDefense >= 0
            ? enemyInfo.Defense - baseInfo.BaseBreakDefense
            : 0; //由于破防超过防御部分无收益，故对破防数值进行预计算
        int defenseSubFull = enemyInfo.Defense - baseInfo.BaseBreakDefense - breakDefense >= 0
            ? enemyInfo.Defense - baseInfo.BaseBreakDefense - breakDefense
            : 0;

        double remainAirShield = CalculateRemainAirShield(baseInfo.BaseBreakAirShield, enemyInfo.AirShield);

        double b1 = (115 * (baseInfo.BaseCriticalHits - enemyInfo.AntiCriticalHits) + 90) * 1.0 /
                    (baseInfo.BaseCriticalHits - enemyInfo.AntiCriticalHits + 940) / 100 +
                    baseInfo.BaseZtCriticalHitsRate * 1.0 / 100; //暴击百分比
        double b2 = CalculateHitRate(baseInfo.BaseHit, enemyInfo.Block, version, fixHitMode); //命中值
        double b3 = (115 * (baseInfo.BaseCriticalHits - enemyInfo.AntiCriticalHits + criticalHits) + 90) * 1.0 /
                    (baseInfo.BaseCriticalHits - enemyInfo.AntiCriticalHits + criticalHits + 940) / 100 +
                    baseInfo.BaseZtCriticalHitsRate * 1.0 / 100; //新增后暴击百分比
        double b4 = CalculateHitRate(baseInfo.BaseHit + hit, enemyInfo.Block, version, fixHitMode); //新增后命中值
        double b5 = b4 - b2; //新增命中百分比
        double b6 = (b2 * (1 + b3 * (baseInfo.BaseCriticalRate * 1.0 / 100 - 1)) + 0.5 - b2 / 2) /
            (b2 * (1 + b1 * (baseInfo.BaseCriticalRate * 1.0 / 100 - 1)) + 0.5 - b2 / 2) - 1; //暴击提升率
        double b7 = (2 * b5 * b1 * (baseInfo.BaseCriticalRate * 1.0 / 100 - 1) + b5) /
                    (2 * b2 * b1 * (baseInfo.BaseCriticalRate * 1.0 / 100 - 1) + b2 + 1); //中间值
        // ReSharper disable once UnusedVariable
        double b8 = (2860 + enemyInfo.Defense) * 1.0 / (2860 + defenseSubBase); //破防收益
        double b9 = (baseInfo.BaseAttack + baseInfo.BaseRestraint - remainAirShield - enemyInfo.AntiRestraint + 920) *
            2860 * 1.0 /
            (2860 + defenseSubBase) + baseInfo.BaseElementAttack - enemyInfo.AntiElementAttack; //等效元素攻击
        double b10 = ((attack + restraint) * 2860 * 1.0 / (2860 + defenseSubBase) + elementAttack) / b9; //新增攻击率
        double b12 = ((baseInfo.BaseAttack + baseInfo.BaseRestraint - remainAirShield - enemyInfo.AntiRestraint + 920) *
                      2860 * 1.0 /
                      (2860 + defenseSubBase) *
                      (2860 + defenseSubBase) / (2860 + defenseSubFull) + baseInfo.BaseElementAttack -
                      enemyInfo.AntiElementAttack) / b9 - 1; //破防收益率
        double b13 =
            (b2 * (1 + b1 * (baseInfo.BaseCriticalRate * 1.0 / 100 + criticalRate * 1.0 / 100 - 1)) + 0.5 - b2 / 2) /
            (b2 * (1 + b1 * (baseInfo.BaseCriticalRate * 1.0 / 100 - 1)) + 0.5 - b2 / 2) - 1; //会伤收益率

        rate.Item1 = (b6 + 1) * (b7 + 1) * (1 + b10) * (1 + b12) * (1 + b13) - 1;
        rate.Item2 = fixHitMode ? Math.Min(1, b4) : b4;

        return rate;
    }

    /// <summary>
    /// 计算每点提升的收益
    /// </summary>
    /// <param name="baseInfo">玩家面板</param>
    /// <param name="enemyInfo">敌方数据</param>
    /// <param name="deliveryData">投放数据，目前为定值</param>
    /// <param name="version">算法版本</param>
    /// <returns>攻击克制,属性攻击,破防,命中,会心,会伤,气海,身法</returns>
    public static KiEarningRate CalculatePerGains(PlayerBaseInfo baseInfo, EnemyInfo enemyInfo,
        DeliveryData deliveryData, HitCalculateVersion version = HitCalculateVersion.Version11)
    {
        KiEarningRate rate = new KiEarningRate
        {
            AttackAndRestraint =
                CalculateIncreaseRate(baseInfo, enemyInfo, attack: deliveryData.Attack + deliveryData.Restraint).Item1 /
                (deliveryData.Attack + deliveryData.Restraint),
            ElementAttack =
                CalculateIncreaseRate(baseInfo, enemyInfo, elementAttack: deliveryData.ElementAttack).Item1 /
                deliveryData.ElementAttack,
            BreakDefense = CalculateIncreaseRate(baseInfo, enemyInfo, breakDefense: deliveryData.BreakDefense).Item1 /
                           deliveryData.BreakDefense,
            Hit = CalculateIncreaseRate(baseInfo, enemyInfo, hit: deliveryData.Hit, fixHitMode: true, version: version)
                      .Item1 /
                  deliveryData.Hit,
            CriticalHits =
                CalculateIncreaseRate(baseInfo, enemyInfo, criticalHits: deliveryData.CriticalHits, version: version)
                    .Item1 /
                deliveryData.CriticalHits,
            CriticalRate = CalculateIncreaseRate(baseInfo, enemyInfo, criticalRate: deliveryData.CriticalRate).Item1 /
                           deliveryData.CriticalRate
        };

        rate.Strength = rate.AttackAndRestraint * 5 + rate.BreakDefense * 2;
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
    /// <param name="breakAirShieldOfPlayer">玩家破盾</param>
    /// <param name="airShieldOfMonster">敌方气盾</param>
    /// <param name="airShieldEnable">是否计算破盾影响</param>
    /// <param name="enemyAntiElement">敌方元素抗性</param>
    public static double CalculateBaseDamage(int attackOfPlayer, int restraintNum, int elementAttackOfPlayer,
        int breakDefenseOfPlayer, int defenseOfMonster, int resistOfMonster, double rate1, double rate2,
        int breakAirShieldOfPlayer = 0, int airShieldOfMonster = 0, bool airShieldEnable = false,
        int enemyAntiElement = 0)
    {
        double remainAirShield =
            airShieldEnable ? CalculateRemainAirShield(breakAirShieldOfPlayer, airShieldOfMonster) : 0;
        double resistanceRemission = enemyAntiElement * 1.0 / (enemyAntiElement + 530); //敌方元素抗性减免
        int remainDefense = defenseOfMonster - breakDefenseOfPlayer; //敌方剩余防御，理论不会小于0，不做特别判断
        if (remainDefense < 0)
        {
            remainDefense = 0;
        }

        double defenseRemission = remainDefense * 1.0 / (remainDefense + 2860); //防御减免

        double baseDamage =
            ((rate1 + rate2 * (attackOfPlayer - remainAirShield + restraintNum - resistOfMonster)) *
                (1 - defenseRemission) + rate2 * elementAttackOfPlayer * (1 - resistanceRemission)); //无首领克制加成的伤害

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
    /// <param name="version">算法版本</param>
    public static double CalculateCriticalDamage(double nonCriticalDamage, int hitNum, int blockOfMonster,
        double criticalSubRate, int criticalHit, int criticalDefenseOfMonster, double extraCriticalRate,
        out double calCriticalRate, HitCalculateVersion version = HitCalculateVersion.Version11)
    {
        double hitRateOfPlayer = CalculateHitRate(hitNum, blockOfMonster, version, false); //玩家对敌方命中率
        if (hitRateOfPlayer > 1)
        {
            hitRateOfPlayer = 1;
        }

        int remainCritical = (criticalHit - criticalDefenseOfMonster); //剩余会心
        double criticalRate =
            (115 * remainCritical + 90) * 1.0 / (remainCritical + 940) / 100 + extraCriticalRate; //会心率
        calCriticalRate = criticalRate;
        double criticalDamage = nonCriticalDamage * hitRateOfPlayer * (1 + criticalSubRate * criticalRate) +
                                0.5 * nonCriticalDamage * (1 - hitRateOfPlayer); //会心伤害【未计算技能倍数】

        return criticalDamage;
    }

    #region 特定数值计算方法

    /// <summary>
    /// 计算命中率（不处理满命中后的情况，需要由调用方自行处理）
    /// </summary>
    /// <param name="fullHit">玩家命中</param>
    /// <param name="block">敌方格挡</param>
    /// <param name="versionEnum">算法版本</param>
    /// <param name="fixMode">最大值是否限制为1，用于兼容部分计算器的情况</param>
    /// <returns>命中率</returns>
    public static double CalculateHitRate(double fullHit, int block, HitCalculateVersion versionEnum, bool fixMode)
    {
        double rate = 0;
        switch (versionEnum)
        {
            case HitCalculateVersion.Version11:
                rate = (95 + 143 * fullHit * 1.0 / (fullHit + 713) - 143 * block * 1.0 / (block + 713)) / 100;
                break;
            case HitCalculateVersion.Version13:
                rate = (95 + 141.9 * (fullHit - block) * 1.0 / (fullHit - block + 3640)) / 100;
                break;
        }

        return fixMode ? Math.Min(1, rate) : rate;
    }

    /// <summary>
    /// 剩余气盾计算方法
    /// </summary>
    /// <param name="breakAirShieldOfPlayer">玩家破盾</param>
    /// <param name="airShieldOfMonster">敌方气盾</param>
    /// <returns>剩余气盾</returns>
    public static double CalculateRemainAirShield(int breakAirShieldOfPlayer, int airShieldOfMonster)
    {
        return breakAirShieldOfPlayer >= airShieldOfMonster
            ? 0
            : breakAirShieldOfPlayer * 1.0 <= airShieldOfMonster * 1.0 / 3
                ? airShieldOfMonster - 2 * breakAirShieldOfPlayer
                : 0.5 * (airShieldOfMonster - breakAirShieldOfPlayer);
    }

    #endregion
}