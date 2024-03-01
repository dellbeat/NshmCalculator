﻿using System.ComponentModel.DataAnnotations;

namespace NshmCalculator.Shared.Models.CalculatorModel;

public class FullPlayerInfo
{
	#region 玩家基础属性

	/// <summary>
	/// 玩家基础攻击力
	/// </summary>
	[Required]
	[Range(1, 100000, ErrorMessage = "请输入1至100000内的整数")]
	public int PlayerBaseAttack { get; set; }

	/// <summary>
	/// 玩家基础破防
	/// </summary>
	[Required]
	[Range(1, 100000, ErrorMessage = "请输入1至100000内的整数")]
	public int PlayerBaseBreakDefense { get; set; }

	/// <summary>
	/// 玩家基础元素攻击
	/// </summary>
	[Required]
	[Range(1, 100000, ErrorMessage = "请输入1至100000内的整数")]
	public int PlayerBaseElementAttack { get; set; }

	/// <summary>
	/// 玩家基础克制
	/// </summary>
	[Required]
	[Range(1, 100000, ErrorMessage = "请输入1至100000内的整数")]
	public int PlayerBaseRestraint { get; set; }

	/// <summary>
	/// 玩家基础命中
	/// </summary>
	[Required]
	[Range(1, 100000, ErrorMessage = "请输入1至100000内的整数")]
	public int PlayerBaseHit { get; set; }

	/// <summary>
	/// 玩家基础会心
	/// </summary>
	[Required]
	[Range(1, 100000, ErrorMessage = "请输入1至100000内的整数")]
	public int PlayerBaseCriticalHits { get; set; }

	/// <summary>
	/// 玩家基础会伤率
	/// </summary>
	[Required]
	[Range(0, 1000, ErrorMessage = "请保证会伤百分比在0.0-1000.0范围内")]
	public double PlayerBaseCriticalRate { get; set; }

	/// <summary>
	/// 玩家周天会心率
	/// </summary>
	[Required]
	[Range(0, 5, ErrorMessage = "请保证周天会心百分比在0.0-5.0范围内")]
	public double PlayerBaseZtCriticalHitsRate { get; set; }

	/// <summary>
	/// 克制百分比
	/// </summary>
	[Required]
	[Range(0, 100, ErrorMessage = "请保证克制百分比在0.0-100.0范围内")]
	public double PlayerBaseRestrainedRate { get; set; }

	#endregion

	#region 新增数值

	/// <summary>
	/// 新增攻击力
	/// </summary>
	[Required]
	[Range(-100000, 100000, ErrorMessage = "请输入-100000至100000内的整数")]
	public int IncreaseAttack { get; set; }

	/// <summary>
	/// 新增破防
	/// </summary>
	[Required]
	[Range(-100000, 100000, ErrorMessage = "请输入-100000至100000内的整数")]
	public int IncreaseBreakDefense { get; set; }

	/// <summary>
	/// 新增元素攻击
	/// </summary>
	[Required]
	[Range(-100000, 100000, ErrorMessage = "请输入-100000至100000内的整数")]
	public int IncreaseElementAttack { get; set; }

	/// <summary>
	/// 新增克制
	/// </summary>
	[Required]
	[Range(-100000, 100000, ErrorMessage = "请输入-100000至100000内的整数")]
	public int IncreaseRestraint { get; set; }

	/// <summary>
	/// 新增命中
	/// </summary>
	[Required]
	[Range(-100000, 100000, ErrorMessage = "请输入-100000至100000内的整数")]
	public int IncreaseHit { get; set; }

	/// <summary>
	/// 新增会心
	/// </summary>
	[Required]
	[Range(-100000, 100000, ErrorMessage = "请输入-100000至100000内的整数")]
	public int IncreaseCriticalHits { get; set; }

	/// <summary>
	/// 新增会伤率
	/// </summary>
	[Required]
	[Range(0, 10, ErrorMessage = "请保证新增会伤百分比在0.0-10.0范围内")]
	public double IncreaseCriticalRate { get; set; }

	/// <summary>
	/// 新增会伤百分比
	/// </summary>
	[Required]
	[Range(0, 5, ErrorMessage = "请保证新增会心百分比在0.0-5.0范围内")]
	public double IncreaseCriticalHitsRate { get; set; }

	/// <summary>
	/// 新增克制百分比
	/// </summary>
	[Required]
	[Range(0, 100, ErrorMessage = "请保证新增克制百分比在0.0-100.0范围内")]
	public double IncreaseRestrainedRate { get; set; }


	#endregion

	#region 敌方数值

	/// <summary>
	/// 敌方防御
	/// </summary>
	[Required]
	[Range(0, 100000, ErrorMessage = "请输入1至100000内的整数")]
	public int EnemyDefense { get; set; }

	/// <summary>
	/// 敌方抵御
	/// </summary>
	[Required]
	[Range(0, 100000, ErrorMessage = "请输入1至100000内的整数")]
	public int EnemyResist { get; set; }

	/// <summary>
	/// 敌方格挡
	/// </summary>
	[Required]
	[Range(0, 100000, ErrorMessage = "请输入1至100000内的整数")]
	public int EnemyBlock { get; set; }

	/// <summary>
	/// 敌方会心抵抗
	/// </summary>
	[Required]
	[Range(0, 100000, ErrorMessage = "请输入1至100000内的整数")]
	public int EnemyCriticalDefense { get; set; }


	/// <summary>
	/// 满命中
	/// </summary>
	public int FullHit => Convert.ToInt32(Math.Floor((105524 * EnemyBlock + 2541845) * 1.0 / (98394 - 5 * EnemyBlock)));

	#endregion

	/// <summary>
	/// 无首领加成的基础伤害
	/// </summary>
	public double BaseDamageFunValue { get; set; }

	/// <summary>
	/// 未会心伤害
	/// </summary>
	public double NonCriticalDamageFunValue { get; set; }

	/// <summary>
	/// 包含命中的伤害期望
	/// </summary>
	public double CriticalDamageFunValue { get; set; }

	/// <summary>
	/// 实际命中率
	/// </summary>
	public double HitRate { get; set; }

	/// <summary>
	/// 实际会心率
	/// </summary>
	public double CalCriticalRate { get; set; }

	/// <summary>
	/// 上次计算时间
	/// </summary>
	public DateTime? LastCalTime { get; set; }

	public FullPlayerInfo()
	{
		PlayerBaseAttack = 2500;
		PlayerBaseRestraint = 1000;
		PlayerBaseElementAttack = 1000;
		PlayerBaseBreakDefense = 1000;
		PlayerBaseHit = 500;
		PlayerBaseCriticalHits = 888;
		PlayerBaseCriticalRate = 188;

		EnemyBlock = 750;
		EnemyDefense = 3000;

        EnemyCriticalDefense = 200;
    }
}